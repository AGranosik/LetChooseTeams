using System;
using System.Threading.Tasks;
using FluentAssertions;
using LCT.Application.Common.Interfaces;
using LCT.Application.Teams.Events.Actions;
using LCT.Application.Tournaments.Hubs;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.IntegrationTests.Mocks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using NUnit.DFM;
using NUnit.Framework;

namespace LCT.IntegrationTests.Repositories.LctActionRepository
{
    [TestFixture]
    public class LctActionRepositoryTests : Testing<Tournament>
    {
        public LctActionRepositoryTests()
        {
            SwapSingleton<IHubContext<TournamentHub>>(IHubContextMock.GetMockedHubContext<TournamentHub>());
            this.Environment("Development")
                .ProjectName("LCT.Api")
                .Build();
        }

        [Test]
        public async Task SaveAsync_Success()
        {
            var repo = GetLctActionRepostiory();
            var id = Guid.NewGuid();
            for (int i = 0; i< 10; i++)
            {
                var action = new TeamClickedAction
                {
                    GroupKey = id,
                    Name = "hehe",
                    Surname = "hehe",
                    Team = "hehe"
                };

                var func = () => repo.SaveAsync(action);
                await func.Should().NotThrowAsync();
            }

            var actions = await repo.GetByGroupIdAsync(id);
            actions.Should().NotBeNull();
            actions.Should().HaveCount(10);
        }

        [Test]
        public async Task GetAsync_NoGroupKey_Success()
        {
            var repo = GetLctActionRepostiory();
            var id = Guid.NewGuid();
            for (int i = 0; i < 10; i++)
            {
                var action = new TeamClickedAction
                {
                    GroupKey = id,
                    Name = "hehe",
                    Surname = "hehe",
                    Team = "hehe"
                };

                var func = () => repo.SaveAsync(action);
                await func.Should().NotThrowAsync();
            }

            var actions = await repo.GetByGroupIdAsync(Guid.NewGuid());
            actions.Should().NotBeNull();
            actions.Should().BeEmpty();
        }



        private ILctActionRepository<TeamClickedAction, Guid> GetLctActionRepostiory()
            => _scope.ServiceProvider.GetRequiredService<ILctActionRepository<TeamClickedAction, Guid>>();
    }
}
