using FluentAssertions;
using LCT.Application.Teams.Commands;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Core.Entities.Tournaments.Types;
using LCT.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NUnit.DFM;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Tournaments.IntegrationTests
{
    [TestFixture]
    public class TournamentSelectTeamTests: Testing<LctDbContext>
    {
        private readonly List<Player> _players = new List<Player>()
        {
            Player.Register("test1", "test2"),
            Player.Register("hehe", "fiu")
        };
        public TournamentSelectTeamTests()
        {
            AddTablesToTruncate(new List<string>
            {
                nameof(LctDbContext.Players),
                nameof(LctDbContext.Tournaments),
                nameof(LctDbContext.SelectedTeams)
            });

            this.Environment("Development")
                .ProjectName("LCT.Api")
                .Build();
        }

        [Test]
        public async Task SelectTeam_FirstTeamSelect()
        {
            var tournament = await CreateTournamentWithPlayers(_players);
            var firstPlayerAssign = await SelectTeamCommandHandler(tournament.Id, TournamentTeamNames._teams.First(), tournament.Players.First().Id);

            firstPlayerAssign.Should().NotBeNull();

            var secondPlayerAssign = await SelectTeamCommandHandler(tournament.Id, TournamentTeamNames._teams.Last(), tournament.Players.Last().Id);

            secondPlayerAssign.Should().NotBeNull();


            var savedTournament = await GetTournamentById(tournament.Id);
            savedTournament.Should().NotBeNull();

            var selectedTeams = savedTournament.SelectedTeams;
            selectedTeams.Should().NotBeNullOrEmpty();

            selectedTeams.Count.Should().Be(2);
            var firstSelectedTeam = selectedTeams.First();
            firstSelectedTeam.Player.Should().Be(_players[0]);
            firstSelectedTeam.TeamName.Value.Should().Be(TournamentTeamNames._teams.First());

            var secondSelectedPlayer = selectedTeams.Last();
            secondSelectedPlayer.Player.Should().Be(_players[1]);
            secondSelectedPlayer.TeamName.Value.Should().Be(TournamentTeamNames._teams.Last());
        }


        [Test]
        public async Task SelectTeam_TournamentDoesNotExist_ThrowsAsync()
        {
            var func = () => SelectTeamCommandHandler(Guid.NewGuid(), "asdasd", Guid.NewGuid());
            await func.Should().ThrowAsync<ArgumentException>();
        }

        private async Task<Unit> SelectTeamCommandHandler(Guid tournamentId, string teamName, Guid playerId)
        {
            return await new SelectTeamCommandHandler(GetDbContext()).Handle(new SelectTeamCommand
            {
                PlayerId = playerId,
                Team = teamName,
                TournamentId = tournamentId
            }, new CancellationToken());
        }

        private async Task<Tournament> GetTournamentById(Guid id)
            => await GetDbContext().Tournaments
            .Include(t => t.Players)
            .Include(t => t.SelectedTeams)
            .FirstOrDefaultAsync(t => t.Id == id);

        private async Task<Tournament> CreateTournamentWithPlayers(List<Player> players)
        {
            var tournament = Tournament.Create(new Name("test"), new TournamentLimit(3));
            foreach (var player in players)
                tournament.AddPlayer(player);

            await AddAsync(tournament);
            return tournament;
        }
    }
}
