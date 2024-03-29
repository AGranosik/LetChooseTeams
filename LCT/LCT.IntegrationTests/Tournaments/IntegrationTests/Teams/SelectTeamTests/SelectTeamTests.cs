﻿using FluentAssertions;
using LCT.Application.Teams.Commands;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Exceptions;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players;
using LCT.Infrastructure.ClientCommunication.Hubs;
using LCT.Infrastructure.Common.Exceptions;
using LCT.IntegrationTests.Mocks;
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
    public class SelectTeamTests : Testing<Tournament>
    {
        public SelectTeamTests()
        {
            AddTableToTruncate("TournamentStream");
            AddTableToTruncate("Tournament_SetTournamentNameEvent_index");
            SwapSingleton(IHubContextMock.GetMockedHubContext<TournamentHub>());
            Environment("Development")
                .ProjectName("LCT.Api")
                .Build();
        }

        [Test]
        public async Task TournamentDoesNotExist_throwsException()
        {
            var func = () => SelectTeamCommandHandlerAsync("hehe", "fiu fiu", Guid.Empty, "hehe");
            await func.Should().ThrowAsync<EntityDoesNotExist>();
        }

        [Test]
        public async Task PlayerHasToBeInTournament_ThrowsException()
        {
            var tournament = await CreateTournament();
            var player = Player.Create("hehe", "222");
            var func = () => SelectTeamCommandHandlerAsync(player.Name, player.Surname, tournament.Id.Value, "hehe");
            await func.Should().ThrowAsync<PlayerNotInTournamentException>();
        }

        [Test]
        public async Task PlayerCannotSelectTwice_ThrowsException()
        {
            var tournament = await CreateTournament();
            var player = tournament.Players.First();
            var func = () => SelectTeamCommandHandlerAsync(player.Name, player.Surname, tournament.Id.Value, TournamentTeamNames.Teams.Last());
            await func.Should().ThrowAsync<PlayerSelectedTeamBeforeException>();
        }

        [Test]
        public async Task TeamCannotBeSelectTwice_ThrowsException()
        {
            var tournament = await CreateTournament();
            var player = tournament.Players.Last();
            var func = () => SelectTeamCommandHandlerAsync(player.Name, player.Surname, tournament.Id.Value, TournamentTeamNames.Teams.Last());
            await func.Should().ThrowAsync<TeamAlreadySelectedException>();
        }

        [Test]
        public async Task TeamSelected_Success()
        {
            var tournament = await CreateTournament();
            var player = tournament.Players.Last();
            var selectTeamAction = () => SelectTeamCommandHandlerAsync(player.Name, player.Surname, tournament.Id.Value, TournamentTeamNames.Teams.First());
            await selectTeamAction.Should().NotThrowAsync();

            var tournamentFromDb = await GetTournamentById(tournament.Id.Value);
            tournamentFromDb.Should().NotBeNull();
            tournamentFromDb.Players.Count().Should().Be(3);
            tournamentFromDb.SelectedTeams.Count().Should().Be(2);
            tournamentFromDb.SelectedTeams.Any(t => t.TeamName == TournamentTeamNames.Teams.First()).Should().BeTrue();
        }

        [Test]
        public async Task TeamSelection_IndexIsSet_ThrowsException()
        {
            var tournament = await CreateTournament();
            var teamToSelect = TournamentTeamNames.Teams.First();

            var playerWhoWillBeBlocked = tournament.Players.Skip(1).First();
            var playerWhoSelectTeamFirst = tournament.Players.Last();

            var teamSubmittedFunc = () => SelectTeamCommandHandlerAsync(playerWhoSelectTeamFirst.Name, playerWhoSelectTeamFirst.Surname, tournament.Id.Value, teamToSelect);
            await teamSubmittedFunc.Should().NotThrowAsync();

            var teamNotSubmitted = () => SelectTeamCommandHandlerAsync(playerWhoWillBeBlocked.Name, playerWhoWillBeBlocked.Surname, tournament.Id.Value, teamToSelect);
            await teamNotSubmitted.Should().ThrowAsync<TeamAlreadySelectedException>();
        }


        [Test]
        public async Task TeamSelection_SameTeamInDifferentTournament_Success()
        {
            var tournament1 = await CreateTournament();
            var tournament2 = await CreateTournament();

            var newTeamToSelect = TournamentTeamNames.Teams.First();
            var player1 = tournament1.Players.Last();
            var firstTournamentSubmit = () => SelectTeamCommandHandlerAsync(player1.Name, player1.Surname, tournament1.Id.Value, newTeamToSelect);
            await firstTournamentSubmit.Should().NotThrowAsync();

            var secondTournamentSubmit = () => SelectTeamCommandHandlerAsync(player1.Name, player1.Surname, tournament2.Id.Value, newTeamToSelect);
            await secondTournamentSubmit.Should().NotThrowAsync();
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
            tournament.AddPlayer(players[2].Name, players[2].Surname);
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
