﻿using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LCT.Application.Tournaments.Commands;
using LCT.Application.Tournaments.Hubs;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Common.BaseTypes;
using LCT.IntegrationTests.Mocks;
using Microsoft.AspNetCore.SignalR;
using NUnit.DFM;
using NUnit.Framework;

namespace LCT.IntegrationTests.Repositories
{
    [TestFixture]
    public class AggregateRepositoryFailureTests : Testing<Tournament>
    {
        public AggregateRepositoryFailureTests()
        {
            SwapSingleton<IHubContext<TournamentHub>>(IHubContextMock.GetMockedHubContext<TournamentHub>());

            var mediatorMock = IMediatorMock.GetMockWithException<DomainEvent>();
            SwapSingleton(mediatorMock);
            this.Environment("Development")
                .ProjectName("LCT.Api")
                .Build();
        }

        [Test]
        public async Task AggregateRepository_MediatorThrowsException_NothingHappen()
        {
            var func = () => CreateTournamenCommandHander(new CreateTournamentCommand
            {
                Name = "hehe",
                PlayerLimit = 3
            });
            await func.Should().NotThrowAsync();
        }


        private async Task<Guid> CreateTournamenCommandHander(CreateTournamentCommand request)
        {
            return await new CreateTournamentCommandHandler(GetRepository(), GetTournamentDomainService()).Handle(request, new CancellationToken());
        }
    }
}
