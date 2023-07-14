using System.Linq.Expressions;
using LCT.Application.Common.Interfaces;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Events;
using LCT.Domain.Common.BaseTypes;
using LCT.Domain.Common.Interfaces;
using LCT.Infrastructure.Persistance.EventsStorage.UniqnessFactories;
using LCT.Infrastructure.Persistance.EventsStorage.UniqnessFactories.Models;
using LCT.Infrastructure.Persistance.SnapshotsStorage;
using MongoDB.Driver;

namespace LCT.Infrastructure.Persistance.EventsStorage
{
    // split into mongo persistance & Events storage which gonna use ISnapshot storage

    public class MongoPersistanceClient : IPersistanceClient
    {
        private readonly IMongoClient _mongoClient;
        private readonly string _dbName;
        private readonly IMongoDatabase _database;
        private readonly IUniqnessIndexExecutor _uniqnessExecutor;
        public MongoPersistanceClient(IMongoClient mongoClient, MongoSettings mongoSettings, IUniqnessIndexExecutor uniqnessExecutor)
        {
            _mongoClient = mongoClient;
            _dbName = mongoSettings.DatabaseName;
            _database = _mongoClient.GetDatabase(_dbName);
            _uniqnessExecutor = uniqnessExecutor;
            Configure();
        }

        public async Task SaveEventAsync<TAggregateRoot>(DomainEvent[] domainEvents, int version = 0)
            where TAggregateRoot : IAgregateRoot, new()
        {
            bool isVersionable = false;
            bool createSnapshot = false;
            var aggregateId = domainEvents[0].StreamId.ToString();
            int latestEventNumber = 0;
            var tasks = new List<Task>(3);


            using var session = await _mongoClient.StartSessionAsync();
            session.StartTransaction();
            foreach (var domainEvent in domainEvents)
            {
                latestEventNumber = domainEvent.EventNumber.Value;
                if (latestEventNumber % 5 == 0)
                    createSnapshot = true;

                if (domainEvent is IVersionable)
                    isVersionable = true;

                if (domainEvent is IUniqness)
                {
                    await _uniqnessExecutor.ExcecuteAsync(session, domainEvent);
                }
            }

            if (isVersionable)
                tasks.Add(Versioning(GetVersionIndex<TAggregateRoot>(), aggregateId, version, session));


            tasks.Add(GetCollection<DomainEvent>(GetStreamName<TAggregateRoot>())
                .InsertManyAsync(session, domainEvents));

            await Task.WhenAll(tasks.ToArray());
            await session.CommitTransactionAsync();

            if (createSnapshot)
                CreateSnapshotAsync<TAggregateRoot>(domainEvents[0].StreamId, latestEventNumber);
        }

        public static string GetStreamName<TAggregate>()
            where TAggregate : IAgregateRoot
            => $"{typeof(TAggregate).Name}_Stream";

        public static string GetVersionIndex<TAggregate>()
            where TAggregate : IAgregateRoot
            => $"{typeof(TAggregate).Name}_Version_index";

        public static string GetSnapshotName<TAggregate>()
            where TAggregate : IAgregateRoot
            => $"{typeof(TAggregate).Name}_Snapshot";
        private async Task Versioning(string aggregateName, string aggregateId, int version, IClientSessionHandle session)
        {
            await GetCollection<AggregateVersionModel>(aggregateName)
                .InsertOneAsync(session, new AggregateVersionModel(aggregateId, version));
        }

        private async Task CreateSnapshotAsync<TAggregateRoot>(Guid streamId, int eventNumber)
            where TAggregateRoot : IAgregateRoot, new()
        {
            var aggregate = await GetAggregateAsync<TAggregateRoot>(streamId);
            var snapshot = new AggregateSnapshot<TAggregateRoot>(eventNumber, aggregate, streamId);
            var snapshotCollection = GetCollection<AggregateSnapshot<TAggregateRoot>>(GetSnapshotName<TAggregateRoot>());

            var snapshotExist = await snapshotCollection
                .FindAsync(a => a.StreamId == streamId);

            if(await snapshotExist.AnyAsync())
            {
                await snapshotCollection.ReplaceOneAsync(a => a.StreamId == streamId, snapshot);
            }
            else
            {
                await snapshotCollection.InsertOneAsync(snapshot);
            }
        }

        public async Task<TAggregateRoot> GetAggregateAsync<TAggregateRoot>(Guid streamId)
            where TAggregateRoot : IAgregateRoot, new()
        {
            var latestsSnapshotCursor = await GetCollection<AggregateSnapshot<TAggregateRoot>>(GetSnapshotName<TAggregateRoot>())
                .FindAsync(a => a.StreamId == streamId);

            var latestSnapshot = await latestsSnapshotCursor.FirstOrDefaultAsync();
            List<DomainEvent> events;
            var snapshotExists = latestSnapshot is not null;
            if (!snapshotExists)
            {
                events = await GetEventsAsync<TAggregateRoot>(streamId);
            }
            else
            {
                events = await GetEventsAsync<TAggregateRoot>(streamId, latestSnapshot.EventNumber);
            }

            if (!snapshotExists && events.Count == 0)
                return default;
            var aggregate = latestSnapshot is null ? new TAggregateRoot() : latestSnapshot.Aggregate;
            aggregate.Load(events);

            return aggregate;
        }

        private IMongoCollection<T> GetCollection<T>(string streamName)
            => _database.GetCollection<T>($"{streamName}");

        private async Task<List<DomainEvent>> GetEventsAsync<T>(Guid streamId, int? eventNumber = null)
            where T: IAgregateRoot, new()
        {
            Expression<Func<DomainEvent, bool>> expression = s => s.StreamId == streamId;
            if (eventNumber.HasValue)
                expression = s => s.StreamId == streamId && s.EventNumber > eventNumber.Value;

            var cursorAsync = await GetCollection<DomainEvent>(GetStreamName<T>()).FindAsync(expression);
            return await cursorAsync.ToListAsync();
        }

        private void Configure()
        {
            var uniqueIndexOptions = new CreateIndexOptions { Unique = true };

            ConfigureFieldUniqness<Tournament, SetTournamentNameEvent>(uniqueIndexOptions);
            ConfigureTournamentTeamSelection(uniqueIndexOptions);
            ConfigureAggregateVersion<Tournament>(uniqueIndexOptions);
        }

        private void ConfigureFieldUniqness<TAggregate, TEventUniqueField>(CreateIndexOptions indexOptions)
            where TEventUniqueField : IUniqness
        {
            _uniqnessExecutor
                .RegisterUniqnessForEvent<TEventUniqueField>(typeof(TAggregate).Name, _database);
        }

        private void ConfigureTournamentTeamSelection(CreateIndexOptions indexOptions)
        {
            var teamSelectionIndex = Builders<TeamSelectionUniqnessModel>.IndexKeys
                .Ascending(l => l.Team)
                .Ascending(l => l.TournamentId);

            var indexModel = new CreateIndexModel<TeamSelectionUniqnessModel>(
                teamSelectionIndex,
                indexOptions);

            _database.GetCollection<TeamSelectionUniqnessModel>("Tournament_SelectedTeams_index")
                .Indexes.CreateOne(indexModel);
        }

        private void ConfigureAggregateVersion<TAggregate>(CreateIndexOptions indexOptions)
            where TAggregate : IAgregateRoot
        {
            var teamSelectionIndex = Builders<AggregateVersionModel>.IndexKeys
                .Ascending(l => l.AggregateId)
                .Ascending(l => l.Version);

            var indexModel = new CreateIndexModel<AggregateVersionModel>(
                teamSelectionIndex,
                indexOptions);

            _database.GetCollection<AggregateVersionModel>(GetVersionIndex<TAggregate>())
                .Indexes.CreateOne(indexModel);
        }
    }
}
