﻿using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LCT.Application.Common.Interfaces;
using LCT.Application.Tournaments.Hubs;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Events;
using LCT.Infrastructure.Persistance.EventsStorage;
using LCT.IntegrationTests.Mocks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NUnit.DFM;
using NUnit.Framework;

namespace LCT.IntegrationTests.Repositories.EventNumber
{
    [TestFixture]
    public class AggregateEventNumberTests : Testing<Tournament>
    {
        private readonly string tournamentStream = MongoPersistanceClient.GetStreamName<Tournament>();
        private IPersistanceClient _client;
        private Tournament _tournament;

        [SetUp]
        public void SetUp()
        {
            _tournament = Tournament.Create("test", 10);
            _client = _scope.ServiceProvider.GetRequiredService<IPersistanceClient>();
        }

        public AggregateEventNumberTests()
        {
            AddTableToTruncate(tournamentStream);
            AddTableToTruncate("Tournament_SetTournamentNameEvent_index");
            SwapSingleton<IHubContext<TournamentHub>>(IHubContextMock.GetMockedHubContext<TournamentHub>());
            this.Environment("Development")
                .ProjectName("LCT.Api")
                .Build();
        }

        [Test]
        public async Task EventNumber_Creation_Success()
        {
            await SaveAsync(_tournament);

            var events = await _client.GetEventsAsync<Tournament>(_tournament.Id.Value);
            events.Should().NotBeNull()
                .And.NotBeEmpty();

            events.Should().HaveCount(2);
            events.All(e => e.EventNumber.HasValue)
                .Should().BeTrue();

            events.Any(e => e.EventNumber == 1)
                .Should().BeTrue();

            events.Any(e => e.EventNumber == 2)
                .Should().BeTrue();
        }

        [Test]
        public async Task EventNumber_AggregateLoadedWithEventNumbersThenSaved_Success()
        {
            await SaveAsync(_tournament, _tournament.Version);
            var repository = GetRepository();
            var aggregateFromDb = await repository.LoadAsync(_tournament.Id.Value);

            aggregateFromDb.AddPlayer("hehe", "hehe");

            await SaveAsync(aggregateFromDb, aggregateFromDb.Version);
            aggregateFromDb = await repository.LoadAsync(_tournament.Id.Value);

            aggregateFromDb.SetName("fiu fiu");
            await SaveAsync(aggregateFromDb, aggregateFromDb.Version);

            var events = await _client.GetEventsAsync<Tournament>(_tournament.Id.Value);
            events.Should().NotBeNull()
                .And.NotBeEmpty();

            events.Should().HaveCount(4);
            events.All(e => e.EventNumber.HasValue)
                .Should().BeTrue();

            var addPlayerEvent = events.Single(e => e is PlayerAddedDomainEvent);
            addPlayerEvent.EventNumber.Should().Be(3);
        }
    }
}