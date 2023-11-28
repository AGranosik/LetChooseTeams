using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LCT.Application.Common.Interfaces;
using LCT.Application.Teams.Events.Actions;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players;
using LCT.Domain.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.DFM;
using NUnit.Framework;

namespace LCT.IntegrationTests.Tournaments.IntegrationTests.Actions
{
    [TestFixture]
    public class TeamClickedActionTests: Testing<Tournament>
    {
        public TeamClickedActionTests()
        {
            AddTableToTruncate("TournamentStream");
            AddTableToTruncate("Tournament_SetTournamentNameEvent_index");
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

            var clientCommunicationServiceMock = new Mock<IClientCommunicationService>();
            await ActionHandle(action, clientCommunicationServiceMock.Object);
            var actions = await GetSavedActions(actionGroupKey);
            actions.Should().NotBeNull();
            actions.Should().NotBeEmpty();
            clientCommunicationServiceMock.Verify(c => c.SendAsync(It.IsAny<string>(), It.IsAny<object?>(), CancellationToken.None));
        }

        private List<TeamClickedAction> CreateActions(int numberOfActions, Guid actionGroupKey)
        {
            var actions = new List<TeamClickedAction>();
            for (int i = 0; i < numberOfActions; i++)
            {
                var player = CreatePlayerForTest(i);
                actions.Add(new TeamClickedAction()
                {
                    GroupKey = actionGroupKey,
                    Team = TournamentTeamNames.Teams[TournamentTeamNames.Teams.Count - 1 - i],
                    Name = player.Name,
                    Surname = player.Surname
                });
            }

            return actions;
        }

        private async Task<Tournament> CreateCompleteTournament(int limit, int players)
        {
            var tournament = Tournament.Create("test", limit);

            for (int i = 0; i < players; i++)
            {
                var player = CreatePlayerForTest(i);
                tournament.AddPlayer(player.Name, player.Surname);
            }
            await SaveAsync(tournament);
            return tournament;
        }

        private Player CreatePlayerForTest(int playerNumer)
            => Player.Create(playerNumer.ToString(), playerNumer.ToString());

        private async Task ActionHandle(TeamClickedAction notification, IClientCommunicationService communcationService = null)
        {
            var repo = GetLctActionRepository();
            communcationService ??= new Mock<IClientCommunicationService>().Object;
            var aggregateRepository = _scope.ServiceProvider.GetRequiredService<IAggregateRepository<Tournament>>();
            var actionHandler = new TeamClickedActionHandler(repo, communcationService);
            await actionHandler.Handle(notification, CancellationToken.None);
        }

        private async Task<List<TeamClickedAction>> GetSavedActions(Guid id)
        {
            var repo = GetLctActionRepository();
            return await repo.GetByGroupIdAsync(id);
        }

        private ILctActionRepository<TeamClickedAction, Guid> GetLctActionRepository()
            => _scope.ServiceProvider.GetRequiredService<ILctActionRepository<TeamClickedAction, Guid>>();
    }
}
