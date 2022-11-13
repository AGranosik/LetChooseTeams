using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players;
using Microsoft.Extensions.DependencyInjection;
using NUnit.DFM;
using NUnit.Framework;
using MongoDB.Driver;
using LCT.Domain.Common.BaseTypes;
using LCT.Domain.Common.Exceptions;
using LCT.Application.Tournaments.Hubs;
using LCT.IntegrationTests.Mocks;
using Microsoft.AspNetCore.SignalR;
using LCT.Infrastructure.Persistance.Mongo;

namespace LCT.IntegrationTests.Repositories
{
    [TestFixture]
    public class AggregateRepositoryTests : Testing<Tournament>
    {
        private readonly string tournamentStream = MongoPersistanceClient.GetStreamName<Tournament>();
        public AggregateRepositoryTests()
        {
            AddTableToTruncate(tournamentStream);
            AddTableToTruncate("Tournament_SetTournamentNameEvent_index");
            SwapSingleton<IHubContext<TournamentHub>>(IHubContextMock.GetMockedHubContext<TournamentHub>());
            this.Environment("Development")
                .ProjectName("LCT.Api")
                .Build();
        }

        [Test]
        public async Task SaveAsync_Success()
        {
            var mongoClient = _scope.ServiceProvider.GetRequiredService<IMongoClient>();
            var tournament = await CreateCompleteTournament(3, 3, 3);

            var eventsCursor = await mongoClient.GetDatabase("Lct_test").GetCollection<DomainEvent>(tournamentStream).FindAsync(ts => ts.StreamId == tournament.Id.Value);
            var events = await eventsCursor.ToListAsync();
            events.Should().NotBeNull();
            events.Should().NotBeEmpty();
        }

        [Test]
        public async Task LoadAsync_AggregateDoestNotExist_ThrowsException()
        {
            var repo = GetRepository();
            var func = () => repo.LoadAsync(Guid.Empty);
            await func.Should().ThrowAsync<EntityDoesNotExist>();
        }

        [Test]
        public async Task LoadAsync_FullyCreateAggregate_Success()
        {
            var tournament = await CreateCompleteTournament(6, 6, 6);
            var repo = GetRepository();
            var tournamentFromDb = await repo.LoadAsync(tournament.Id.Value);
            tournamentFromDb.Should().NotBeNull();
        }

        private async Task<Tournament> CreateCompleteTournament(int limit, int players, int selectedTeams)
        {
            var tournament = Tournament.Create("test", limit);

            for (int i = 0; i < players; i++)
            {
                var player = Player.Create(i.ToString(), i.ToString());
                tournament.AddPlayer(player.Name, player.Surname);
            }

            for (int i = 0; i < selectedTeams; i++)
            {
                var player = tournament.Players.ElementAt(i);

                tournament.SelectTeam(player.Name, player.Surname, TournamentTeamNames.Teams[i]);
            }
            await SaveAsync(tournament);
            return tournament;
        }
    }
}
