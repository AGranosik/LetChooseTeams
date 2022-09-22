using System;
using System.Threading;
using System.Threading.Tasks;
using LCT.Application.Teams.Events.Actions;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using Microsoft.Extensions.DependencyInjection;
using NUnit.DFM;
using MongoDB.Driver;
using NUnit.Framework;
using System.Collections.Generic;
using FluentAssertions;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using System.Linq;
using LCT.Application.Common.Interfaces;

namespace LCT.IntegrationTests.Tournaments.IntegrationTests.Teams.Actions
{
    [TestFixture]
    internal class TeamClickedActionHandlerTests: Testing<Tournament>
    {
        public TeamClickedActionHandlerTests()
        {
            this.Environment("Development")
                .ProjectName("LCT.Api")
                .Build();
        }

        [Test]
        public async Task TeamClicked_EmptyName_NotSaved()
        {
            var tournamentId = Guid.NewGuid();
            var action = new TeamClickedAction
            {
                GroupKey = tournamentId
            };

            await TeamClickedActionHandlerAsync(action);
            var actions = await GetClicksForTournament(tournamentId);
            actions.Should().NotBeNull();
            actions.Should().BeEmpty();
        }

        [Test]
        public async Task TeamClicked_TeamDoesNotExist_NotSaved()
        {
            var tournamentId = Guid.NewGuid();
            var action = new TeamClickedAction
            {
                GroupKey = tournamentId,
                Team = "hehe"
            };

            await TeamClickedActionHandlerAsync(action);
            var actions = await GetClicksForTournament(tournamentId);
            actions.Should().NotBeNull();
            actions.Should().BeEmpty();
        }

        [Test]
        public async Task TeamCliecked_ActionSaved_Success()
        {
            var tournamentId = Guid.NewGuid();
            var action = new TeamClickedAction
            {
                GroupKey = tournamentId,
                Team = TournamentTeamNames.Teams.First()
            };

            await TeamClickedActionHandlerAsync(action);
            var actions = await GetClicksForTournament(tournamentId);
            actions.Should().NotBeNull();
            actions.Count.Should().Be(1);
        }

        private async Task TeamClickedActionHandlerAsync(TeamClickedAction action)
        {
            var repository = _scope.ServiceProvider.GetRequiredService<ILctActionRepository<TeamClickedAction>>();
            await new TeamClickedActionHandler(repository).Handle(action, CancellationToken.None);
        }

        private async Task<List<TeamClickedAction>> GetClicksForTournament(Guid tournamentId)
        {
            var persistanceClient = _scope.ServiceProvider.GetRequiredService<IPersistanceClient>();
            var resultCursor = await persistanceClient.GetCollection<TeamClickedAction>(nameof(TeamClickedAction))
                .FindAsync(a => a.GroupKey == tournamentId);

            return await resultCursor.ToListAsync();
        }

        private async Task<Tournament> CreateTournament()
        {
            var tournament = Tournament.Create("test", 3);
            await AddAsync(tournament);
            return tournament;
        }



    }
}
