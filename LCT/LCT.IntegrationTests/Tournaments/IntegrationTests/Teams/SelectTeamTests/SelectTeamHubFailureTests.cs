using FluentAssertions;
using LCT.Application.Common.Interfaces;
using LCT.Application.Teams.Commands;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players;
using LCT.Infrastructure.ClientCommunication.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.DFM;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Tournaments.IntegrationTests.Teams.SelectTeamTests
{
    [TestFixture]
    public class SelectTeamHubFailureTests : Testing<Tournament>
    {
        private Mock<IClientCommunicationService> _clientCommunicationService = new Mock<IClientCommunicationService>();
        public SelectTeamHubFailureTests()
        {
            _clientCommunicationService.Setup(cs => cs.SendAsync(It.IsAny<string>(), It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception());

            SwapSingleton(_clientCommunicationService.Object);
            Environment("Development")
                .ProjectName("LCT.Api")
                .Build();
        }

        [Test]
        public async Task TeamSelectedDespiteHubException()
        {
            var tournament = await CreateTournament();
            var player = tournament.Players.Last();
            var firstPlayerAssignAction = () =>SelectTeamCommandHandlerAsync(player.Name, player.Surname, tournament.Id.Value, TournamentTeamNames.Teams.First());

            await firstPlayerAssignAction.Should().NotThrowAsync();

            var savedTournament = await GetTournamentById(tournament.Id.Value);
            savedTournament.Should().NotBeNull();

            var selectedTeams = savedTournament.SelectedTeams;
            selectedTeams.Should().NotBeNullOrEmpty();

            selectedTeams.Count.Should().Be(2);
            var firstSelectedTeam = selectedTeams.Last();
            firstSelectedTeam.Player.Name.Should().Be(player.Name);
            firstSelectedTeam.Player.Surname.Should().Be(player.Surname);
            firstSelectedTeam.TeamName.Value.Should().Be(TournamentTeamNames.Teams.First());

            _clientCommunicationService.Verify(hc => hc.SendAsync(It.IsAny<string>(), It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.AtLeast(4)); // x2 add player // x2 selected team
        }


        private async Task<Tournament> GetTournamentById(Guid id)
            => await GetRepository().LoadAsync(id);

        private async Task<Tournament> CreateTournament()
        {
            var tournament = Tournament.Create("test", 3);
            var players = new List<Player>()
            {
                Player.Create("1", "2"),
                Player.Create("3", "4"),
                Player.Create("5", "4")
            };
            tournament.AddPlayer(players[0].Name, players[0].Surname);
            tournament.AddPlayer(players[1].Name, players[1].Surname);
            tournament.SelectTeam(players[0].Name, players[0].Surname, TournamentTeamNames.Teams.Last());
            await SaveAsync(tournament);
            return tournament;
        }

        private async Task SelectTeamCommandHandlerAsync(string playerName, string playerSurname, Guid TournamentId, string Team)
        {
            await new SelectTeamCommandHandler(GetRepository()).Handle(new SelectTeamCommand
            {
                PlayerName = playerName,
                PlayerSurname = playerSurname,
                Team = Team,
                TournamentId = TournamentId
            }, new CancellationToken());
        }
    }
}
