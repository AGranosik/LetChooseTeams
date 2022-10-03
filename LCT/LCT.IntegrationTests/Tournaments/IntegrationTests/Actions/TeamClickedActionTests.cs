using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LCT.Application.Common.Interfaces;
using LCT.Application.Teams.Events.Actions;
using LCT.Application.Tournaments.Hubs;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using LCT.IntegrationTests.Mocks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using NUnit.DFM;
using NUnit.Framework;

namespace LCT.IntegrationTests.Tournaments.IntegrationTests.Actions
{
    [TestFixture]
    public class TeamClickedActionTests: Testing<Tournament>
    {
        public TeamClickedActionTests()
        {
            SwapSingleton<IHubContext<TournamentHub>>(IHubContextMock.GetMockedHubContext<TournamentHub>());
            this.Environment("Development")
                .ProjectName("LCT.Api")
                .Build();
        }

        [Test]
        public async Task TeamClicked_TeamNameCannotBeEmpty_NothingSaved()
        {
            var actionGroupKey = Guid.NewGuid();
            var action = new TeamClickedAction()
            {
                GroupKey = actionGroupKey
            };

            await ActionHandle(action);
            var actions = await GetSavedActions(actionGroupKey);
            actions.Should().NotBeNull();
            actions.Should().BeEmpty();
        }

        [Test]
        public async Task TeamClicked_TeamNameDoestNotExist_NothingSaved()
        {
            var actionGroupKey = Guid.NewGuid();
            var action = new TeamClickedAction()
            {
                GroupKey = actionGroupKey,
                Team = "sdsfdg"
            };

            await ActionHandle(action);
            var actions = await GetSavedActions(actionGroupKey);
            actions.Should().NotBeNull();
            actions.Should().BeEmpty();
        }


        [Test]
        public async Task TeamClicked_Success()
        {
            var actionGroupKey = Guid.NewGuid();
            var action = new TeamClickedAction()
            {
                GroupKey = actionGroupKey,
                Team = TournamentTeamNames.Teams.First()
            };

            await ActionHandle(action);
            var actions = await GetSavedActions(actionGroupKey);
            actions.Should().NotBeNull();
            actions.Should().NotBeEmpty();
        }

        private async Task ActionHandle(TeamClickedAction notification)
        {
            var actionHandler = _scope.ServiceProvider.GetRequiredService<INotificationHandler<TeamClickedAction>>();
            await actionHandler.Handle(notification, CancellationToken.None);
        }

        private async Task<List<TeamClickedAction>> GetSavedActions(Guid id)
        {
            var repo = _scope.ServiceProvider.GetRequiredService<ILctActionRepository<TeamClickedAction, Guid>>();
            return await repo.GetByGroupIdAsync(id);
        }
    }
}
