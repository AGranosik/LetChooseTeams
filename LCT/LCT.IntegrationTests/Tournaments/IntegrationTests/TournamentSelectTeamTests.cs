using FluentAssertions;
using LCT.Application.Teams.Commands;
using LCT.Application.Teams.Events;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entities.Tournaments.Types;
using LCT.IntegrationTests.Mocks;
using MediatR;
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
    public class TournamentSelectTeamTests : Testing<Tournament>
    {
        private readonly List<Player> _players = new List<Player>()
        {
            Player.Register("test1", "test2", Guid.NewGuid()),
            Player.Register("hehe", "fiu", Guid.NewGuid())
        };
        public TournamentSelectTeamTests()
        {
            this.Environment("Development")
                .ProjectName("LCT.Api")
                .Build();
        }

        [Test]
        public async Task SelectTeam_FirstTeamSelect()
        {
            var tournament = await CreateTournamentWithPlayers(_players);
            var firstPlayerId = tournament.Players.First().Id;
            var secondPLayerId = tournament.Players.Last().Id;
            var firstPlayerAssign = await SelectTeamCommandHandler(tournament.Id, TournamentTeamNames.Teams.First(), firstPlayerId, IMediatorMock.GetMock());

            firstPlayerAssign.Should().NotBeNull();

            var secondPlayerAssign = await SelectTeamCommandHandler(tournament.Id, TournamentTeamNames.Teams.Last(), secondPLayerId, IMediatorMock.GetMock());
            secondPlayerAssign.Should().NotBeNull();

            var savedTournament = await GetTournamentById(tournament.Id);
            savedTournament.Should().NotBeNull();

            var selectedTeams = savedTournament.SelectedTeams;
            selectedTeams.Should().NotBeNullOrEmpty();

            selectedTeams.Count.Should().Be(2);
            var firstSelectedTeam = selectedTeams.First();
            firstSelectedTeam.Player.Id.Should().Be(firstPlayerId);
            firstSelectedTeam.TeamName.Value.Should().Be(TournamentTeamNames.Teams.First());

            var secondSelectedPlayer = selectedTeams.Last();
            secondSelectedPlayer.Player.Id.Should().Be(secondPLayerId);
            secondSelectedPlayer.TeamName.Value.Should().Be(TournamentTeamNames.Teams.Last());
        }

        [Test]
        public async Task TeamSelectedDespiteHubException()
        {
            var tournament = await CreateTournamentWithPlayers(_players);
            var playerId = tournament.Players.First().Id;
            var firstPlayerAssign = await SelectTeamCommandHandler(tournament.Id, TournamentTeamNames.Teams.First(), playerId, IMediatorMock.GetMockWithException<TeamSelectedMessageEvent>());

            firstPlayerAssign.Should().NotBeNull();

            var savedTournament = await GetTournamentById(tournament.Id);
            savedTournament.Should().NotBeNull();

            var selectedTeams = savedTournament.SelectedTeams;
            selectedTeams.Should().NotBeNullOrEmpty();

            selectedTeams.Count.Should().Be(1);
            var firstSelectedTeam = selectedTeams.First();
            firstSelectedTeam.Player.Id.Should().Be(playerId);
            firstSelectedTeam.TeamName.Value.Should().Be(TournamentTeamNames.Teams.First());
        }


        [Test]
        public async Task SelectTeam_TournamentDoesNotExist_ThrowsAsync()
        {
            var func = () => SelectTeamCommandHandler(Guid.NewGuid(), "asdasd", Guid.NewGuid(), IMediatorMock.GetMock());
            await func.Should().ThrowAsync<ArgumentException>();
        }

        private async Task<Unit> SelectTeamCommandHandler(Guid tournamentId, string teamName, Guid playerId, IMediator mediatorMock)
        {
            return await new SelectTeamCommandHandler(GetRepository(), mediatorMock).Handle(new SelectTeamCommand
            {
                PlayerId = playerId,
                Team = teamName,
                TournamentId = tournamentId
            }, new CancellationToken());
        }

        private async Task<Tournament> GetTournamentById(Guid id)
        => await GetRepository().Load(id);

        private async Task<Tournament> CreateTournamentWithPlayers(List<Player> players)
        {
            var tournament = Tournament.Create("test", 3);
            await AddAsync(tournament);
            foreach (var player in players)
            {
                tournament.AddPlayer(player.Name, player.Surname);
                await AddAsync(tournament);
            
            }

            return tournament;
        }
    }
}
