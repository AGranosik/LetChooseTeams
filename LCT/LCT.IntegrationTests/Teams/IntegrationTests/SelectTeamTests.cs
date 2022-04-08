using FluentAssertions;
using LCT.Application.Teams.Commands;
using LCT.Application.Tournaments.Hubs;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.Exceptions;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Core.Entities.Tournaments.Types;
using LCT.Infrastructure.EF;
using LCT.IntegrationTests.Mocks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NUnit.DFM;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Teams.IntegrationTests
{
    [TestFixture]
    public class SelectTeamTests: Testing<LctDbContext>
    {
        public SelectTeamTests()
        {
            AddTablesToTruncate(new List<string>
            {
                nameof(LctDbContext.SelectedTeams),
                nameof(LctDbContext.Players),
                nameof(LctDbContext.Tournaments)
            });

            this.Environment("Development")
                .ProjectName("LCT.Api")
                .Build();
        }

        [Test]
        public async Task TournamentDoesNotExist_throwsException()
        {
            var func = () => SelectTeamCommandHandlerAsync(Guid.NewGuid(), Guid.NewGuid(), "hehe");
            await func.Should().ThrowAsync<ArgumentException>();
        }

        [Test]
        public async Task PlayerHasToBeInTournament_ThrowsException()
        {
            var tournament = await CreateTournament();
            var player = Player.Register(new Name("hehe"), new Name("222"));
            var func = () => SelectTeamCommandHandlerAsync(player.Id, tournament.Id, "hehe");
            await func.Should().ThrowAsync<PlayerNotInTournamentException>();
        }

        [Test]
        public async Task PlayerCannotSelectTwice_ThrowsException()
        {
            var tournament = await CreateTournament();
            var func = () => SelectTeamCommandHandlerAsync(tournament.Players.First().Id, tournament.Id, TournamentTeamNames._teams.Last());
            await func.Should().ThrowAsync<PlayerSelectedTeamBeforeException>();
        }

        [Test]
        public async Task TeamCannotBeSelectTwice_ThrowsException()
        {
            var tournament = await CreateTournament();
            var func = () => SelectTeamCommandHandlerAsync(tournament.Players.Last().Id, tournament.Id, TournamentTeamNames._teams.Last());
            await func.Should().ThrowAsync<TeamAlreadySelectedException>();
        }

        [Test]
        public async Task TeamSelected_Success()
        {
            var tournament = await CreateTournament();
            var result = await SelectTeamCommandHandlerAsync(tournament.Players.Last().Id, tournament.Id, TournamentTeamNames._teams.First());
            result.Should().NotBeNull();
            var tournamentFromDb = await GetTournamentById(tournament.Id);
            tournamentFromDb.Should().NotBeNull();
            tournamentFromDb.Players.Count().Should().Be(2);
            tournamentFromDb.SelectedTeams.Count().Should().Be(2);
            tournamentFromDb.SelectedTeams.Any(t => t.TeamName == TournamentTeamNames._teams.First()).Should().BeTrue();
        }

        private async Task<Tournament> GetTournamentById(Guid id)
            => await GetDbContext().Tournaments
            .Include(t => t.Players)
            .Include(t => t.SelectedTeams)
            .SingleOrDefaultAsync(t => t.Id == id);

        private async Task<Tournament> CreateTournament()
        {
            var tournament = Tournament.Create(new Name("test"), new TournamentLimit(3));
            var players = new List<Player>()
            {
                Player.Register(new Name("1"), new Name("2")),
                Player.Register(new Name("3"), new Name("4"))
            };
            tournament.AddPlayer(players[0]);
            tournament.AddPlayer(players[1]);
            tournament.SelectTeam(players[0].Id, TournamentTeamNames._teams.Last());
            await AddAsync(tournament);
            return tournament;
        }

        private async Task<Unit> SelectTeamCommandHandlerAsync(Guid playerId, Guid TournamentId, string Team, IHubContext<PlayerAssignedHub> hub = null)
        {
            return await new SelectTeamCommandHandler(GetDbContext(), hub ?? IHubContextMock.GetMockedHubContext<PlayerAssignedHub>()).Handle(new SelectTeamCommand
            {
                PlayerId = playerId,
                Team = Team,
                TournamentId = TournamentId
            }, new CancellationToken());
        }
    }
}
