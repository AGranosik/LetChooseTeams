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

            using var session = _mongoClient.StartSession();
            session.StartTransaction();
            foreach (var domainEvent in domainEvents)
            {
                latestEventNumber = domainEvent.EventNumber.Value;
                if (latestEventNumber % 5 == 0)
                    createSnapshot = true;

                if (domainEvent is IVersionable)
                    isVersionable = true;

                if (domainEvent is IUniqness)
                    _uniqnessExecutor.Excecute(session, domainEvent);
            }

            if (isVersionable)
                Versioning(GetVersionIndex<TAggregateRoot>(), aggregateId, version, session);

            GetCollection<DomainEvent>(GetStreamName<TAggregateRoot>())
                .InsertMany(session, domainEvents);

            await session.CommitTransactionAsync();

            if (createSnapshot)
                await CreateSnapshotAsync<TAggregateRoot>(domainEvents[0].StreamId, latestEventNumber);
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

        private void Versioning(string aggregateName, string aggregateId, int version, IClientSessionHandle session)
            => GetCollection<AggregateVersionModel>(aggregateName)
                .InsertOne(session, new AggregateVersionModel(aggregateId, version));

        private async Task CreateSnapshotAsync<TAggregateRoot>(Guid streamId, int eventNumber)
            where TAggregateRoot : IAgregateRoot, new()
        {
            var aggregate = await GetAggregateAsync<TAggregateRoot>(streamId);
            var snapshot = new AggregateSnapshot<TAggregateRoot>(eventNumber, aggregate, streamId);
            var snapshotCollection = GetCollection<AggregateSnapshot<TAggregateRoot>>(GetSnapshotName<TAggregateRoot>());

            var snapshotExist = await snapshotCollection
                .Find(a => a.StreamId == streamId)
                .AnyAsync();

            if(snapshotExist)
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
            List<DomainEvent> events;
            var latestSnapshot = await GetCollection<AggregateSnapshot<TAggregateRoot>>(GetSnapshotName<TAggregateRoot>())
                    .Find(a => a.StreamId == streamId)
                    .FirstOrDefaultAsync();

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

        private Task<List<DomainEvent>> GetEventsAsync<T>(Guid streamId, int eventNumber = -1)
            where T: IAgregateRoot, new()
             => GetCollection<DomainEvent>(GetStreamName<T>()).Find(s => s.StreamId == streamId && s.EventNumber > eventNumber).ToListAsync();

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
