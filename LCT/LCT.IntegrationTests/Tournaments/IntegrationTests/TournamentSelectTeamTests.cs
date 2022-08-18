using FluentAssertions;
using LCT.Application.Teams.Commands;
using LCT.Application.Teams.Events;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects;
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
            Player.Create("test1", "test2"),
            Player.Create("hehe", "fiu")
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
            var firstPlayer = tournament.Players.First();
            var secondPlayer = tournament.Players.Last();
            var firstPlayerAssign = await SelectTeamCommandHandler(tournament.Id, TournamentTeamNames.Teams.First(), firstPlayer.Name, firstPlayer.Surname, IMediatorMock.GetMock());

            firstPlayerAssign.Should().NotBeNull();

            var secondPlayerAssign = await SelectTeamCommandHandler(tournament.Id, TournamentTeamNames.Teams.Last(), secondPlayer.Name, secondPlayer.Surname, IMediatorMock.GetMock());
            secondPlayerAssign.Should().NotBeNull();

            var savedTournament = await GetTournamentById(tournament.Id);
            savedTournament.Should().NotBeNull();

            var selectedTeams = savedTournament.SelectedTeams;
            selectedTeams.Should().NotBeNullOrEmpty();

            selectedTeams.Count.Should().Be(2);
            var firstSelectedTeam = selectedTeams.First();
            firstSelectedTeam.Player.Name.Should().Be(firstPlayer.Name);
            firstSelectedTeam.Player.Surname.Should().Be(firstPlayer.Surname);
            firstSelectedTeam.TeamName.Value.Should().Be(TournamentTeamNames.Teams.First());

            var secondSelectedPlayer = selectedTeams.Last();
            secondSelectedPlayer.Player.Name.Should().Be(secondPlayer.Name);
            secondSelectedPlayer.Player.Surname.Should().Be(secondPlayer.Surname);
            secondSelectedPlayer.TeamName.Value.Should().Be(TournamentTeamNames.Teams.Last());
        }

        [Test]
        public async Task TeamSelectedDespiteHubException()
        {
            var tournament = await CreateTournamentWithPlayers(_players);
            var player = tournament.Players.First();
            var firstPlayerAssign = await SelectTeamCommandHandler(tournament.Id, TournamentTeamNames.Teams.First(), player.Name, player.Surname, IMediatorMock.GetMockWithException<TeamSelectedMessageEvent>());

            firstPlayerAssign.Should().NotBeNull();

            var savedTournament = await GetTournamentById(tournament.Id);
            savedTournament.Should().NotBeNull();

            var selectedTeams = savedTournament.SelectedTeams;
            selectedTeams.Should().NotBeNullOrEmpty();

            selectedTeams.Count.Should().Be(1);
            var firstSelectedTeam = selectedTeams.First();
            firstSelectedTeam.Player.Name.Should().Be(player.Name);
            firstSelectedTeam.Player.Surname.Should().Be(player.Surname);
            firstSelectedTeam.TeamName.Value.Should().Be(TournamentTeamNames.Teams.First());
        }


        [Test]
        public async Task SelectTeam_TournamentDoesNotExist_ThrowsAsync()
        {
            var func = () => SelectTeamCommandHandler(Guid.NewGuid(), "asdasd", "hehe", "fiu fiu", IMediatorMock.GetMock());
            await func.Should().ThrowAsync<ArgumentException>();
        }

        private async Task<Unit> SelectTeamCommandHandler(Guid tournamentId, string teamName, string playerName, string playerSurname, IMediator mediatorMock)
        {
            return await new SelectTeamCommandHandler(GetRepository(), mediatorMock).Handle(new SelectTeamCommand
            {
                PlayerName = playerName,
                PlayerSurname = playerSurname,
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
