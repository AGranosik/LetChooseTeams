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
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players;
using LCT.Domain.Common.Interfaces;
using LCT.IntegrationTests.Mocks;
using Microsoft.AspNetCore.SignalR;
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

            var hubMocks = HubMocks();
            await ActionHandle(action, hubMocks.Item2.Object);
            var actions = await GetSavedActions(actionGroupKey);
            actions.Should().NotBeNull();
            actions.Should().NotBeEmpty();
            hubMocks.Item1.Verify(c => c.SendCoreAsync(It.IsAny<string>(), It.Is<object?[]>(x => x.Length == 1), CancellationToken.None));
        }

        [Test]
        public async Task TeamClickedByMultiplePlayers_OnePlayerAlreadySelectedTeam_ReturnWithoutSelectedTeam()
        {
            var tournament = await CreateCompleteTournament(3, 3);
            var actionGroupKey = tournament.Id.Value;
            var hubMocks = HubMocks();

            var actions = CreateActions(3, actionGroupKey);

            for(int i = 0; i < 2; i++)
                await ActionHandle(actions[i], hubMocks.Item2.Object);

            var firstPLayer = tournament.Players.First();
            tournament.SelectTeam(firstPLayer.Name, firstPLayer.Surname, TournamentTeamNames.Teams[0]);
            await SaveAsync(tournament);

            hubMocks = HubMocks();
            await ActionHandle(actions[2], hubMocks.Item2.Object);
            var actionsFromDB = await GetSavedActions(actionGroupKey);
            actionsFromDB.Should().NotBeNull();
            actionsFromDB.Count.Should().Be(3);
            var clickedTeam = TournamentTeamNames.Teams.Last();
            actionsFromDB.Any(a => a.Team == clickedTeam).Should().BeTrue();
            hubMocks.Item1.Verify(c => c.SendCoreAsync(It.IsAny<string>(), It.Is<object?[]>(x => ((TeamClickedEvent)x[0]).ClickedTeams.Count == 2 && !((TeamClickedEvent)x[0]).ClickedTeams.Any(ct => ct.Team == TournamentTeamNames.Teams.Last())), CancellationToken.None));

        }

        [Test]
        public async Task TeamClickedByMultiplePlayers_AllPlayersSelectedTeams_SendEmptyList()
        {
            var tournament = await CreateCompleteTournament(3, 3);
            var actionGroupKey = tournament.Id.Value;
            var hubMocks = HubMocks();
            var actions = CreateActions(3, actionGroupKey);

            for (int i = 0; i < 2; i++)
            {
                var action = actions[i];
                await ActionHandle(action, hubMocks.Item2.Object);
                tournament.SelectTeam(action.Name, action.Surname, TournamentTeamNames.Teams[i]);
            }
            await SaveAsync(tournament);
            hubMocks = HubMocks();
            await ActionHandle(actions[2], hubMocks.Item2.Object);
            var actionsFromDB = await GetSavedActions(actionGroupKey);
            actionsFromDB.Should().NotBeNull();
            actionsFromDB.Count.Should().Be(3);
            hubMocks.Item1.Verify(c => c.SendCoreAsync(It.IsAny<string>(), It.Is<object?[]>(x => ((TeamClickedEvent)x[0]).ClickedTeams.Count == 1 && ((TeamClickedEvent)x[0]).ClickedTeams.Any(ct => ct.Team == TournamentTeamNames.Teams[TournamentTeamNames.Teams.Count - 3])), CancellationToken.None));
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

        private Tuple<Mock<IClientProxy>, IMock<IHubContext<TournamentHub>>> HubMocks()
        {
            var hubContext = new Mock<IHubContext<TournamentHub>>();
            var hubClients = new Mock<IHubClients>();
            var clientProxy = new Mock<IClientProxy>();
            hubClients.Setup(hc => hc.All)
                .Returns(clientProxy.Object);
            hubContext.Setup(hc => hc.Clients)
                .Returns(hubClients.Object);

            return new Tuple<Mock<IClientProxy>, IMock<IHubContext<TournamentHub>>>(clientProxy, hubContext);
        }

        private async Task ActionHandle(TeamClickedAction notification, IHubContext<TournamentHub> hub = null)
        {
            var repo = GetLctActionRepository();
            hub ??= IHubContextMock.GetMockedHubContext<TournamentHub>();
            var aggregateRepository = _scope.ServiceProvider.GetRequiredService<IAggregateRepository<Tournament>>();
            var actionHandler = new TeamClickedActionHandler(repo, hub, aggregateRepository);
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
