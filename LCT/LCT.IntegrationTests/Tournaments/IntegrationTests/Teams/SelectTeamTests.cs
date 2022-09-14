using FluentAssertions;
using LCT.Application.Teams.Commands;
using LCT.Application.Teams.Events;
using LCT.Application.Tournaments.Hubs;
using LCT.Core.Shared.Exceptions;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Exceptions;
using LCT.Domain.Aggregates.TournamentAggregate.Services;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players;
using LCT.IntegrationTests.Mocks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using NUnit.DFM;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Tournaments.IntegrationTests.Teams
{
    [TestFixture]
    public class SelectTeamTests : Testing<Tournament>
    {
        public SelectTeamTests()
        {
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
            var result = await SelectTeamCommandHandlerAsync(player.Name, player.Surname, tournament.Id.Value, TournamentTeamNames.Teams.First());
            result.Should().NotBeNull();
            var tournamentFromDb = await GetTournamentById(tournament.Id.Value);
            tournamentFromDb.Should().NotBeNull();
            tournamentFromDb.Players.Count().Should().Be(2);
            tournamentFromDb.SelectedTeams.Count().Should().Be(2);
            tournamentFromDb.SelectedTeams.Any(t => t.TeamName == TournamentTeamNames.Teams.First()).Should().BeTrue();
        }

        [Test]
        public async Task TeamSelection_ConcurrencyIsSet_ThrowsException()
        {
            var tournament = await CreateTournament();
            var teamToSelect = TournamentTeamNames.Teams.First();

            var playerWhoWillBeBlocked = tournament.Players.Skip(1).First();
            var playerWhoSelectTeamFirst = tournament.Players.Last();

            var teamSubmittedFunc = () => SelectTeamCommandHandlerAsync(playerWhoSelectTeamFirst.Name, playerWhoSelectTeamFirst.Surname, tournament.Id.Value, teamToSelect);
            await teamSubmittedFunc.Should().NotThrowAsync();

            var domainService = _scope.ServiceProvider.GetRequiredService<ITournamentDomainService>();
            var uniqnessFunc = () => domainService.PlayerTeamSelectionValidationAsync(teamToSelect, tournament.Id.Value);
            await uniqnessFunc.Should().ThrowAsync<PlayerAlreadyAssignedToTournamentException>();
        }

        [Test]
        public async Task TeamSelectedDespiteHubException()
        {
            var tournament = await CreateTournament();
            var player = tournament.Players.Last();
            var firstPlayerAssign = await SelectTeamCommandHandlerAsync(player.Name, player.Surname, tournament.Id.Value, TournamentTeamNames.Teams.First(), IMediatorMock.GetMockWithException<TeamSelectedMessageEvent>());

            firstPlayerAssign.Should().NotBeNull();

            var savedTournament = await GetTournamentById(tournament.Id.Value);
            savedTournament.Should().NotBeNull();

            var selectedTeams = savedTournament.SelectedTeams;
            selectedTeams.Should().NotBeNullOrEmpty();

            selectedTeams.Count.Should().Be(2);
            var firstSelectedTeam = selectedTeams.Last();
            firstSelectedTeam.Player.Name.Should().Be(player.Name);
            firstSelectedTeam.Player.Surname.Should().Be(player.Surname);
            firstSelectedTeam.TeamName.Value.Should().Be(TournamentTeamNames.Teams.First());
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
            await AddAsync(tournament);
            return tournament;
        }

        private async Task<Unit> SelectTeamCommandHandlerAsync(string playerName, string playerSurname, Guid TournamentId, string Team, IMediator mediatorMock = null)
        {
            var domainService = _scope.ServiceProvider.GetRequiredService<ITournamentDomainService>();
            return await new SelectTeamCommandHandler(GetRepository(), mediatorMock ?? IMediatorMock.GetMock(), domainService).Handle(new SelectTeamCommand
            {
                PlayerName = playerName,
                PlayerSurname = playerSurname,
                Team = Team,
                TournamentId = TournamentId
            }, new CancellationToken());
        }
    }
}
