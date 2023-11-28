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

        public async Task SaveEventAsync<TAggregateRoot>(DomainEvent[] domainEvents, int version = 0, CancellationToken cancellationToken = default(CancellationToken))
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
                Versioning(GetVersionIndex<TAggregateRoot>(), aggregateId, version, session, cancellationToken);

            GetCollection<DomainEvent>(GetStreamName<TAggregateRoot>())
                .InsertMany(session, domainEvents, cancellationToken: cancellationToken);

            await session.CommitTransactionAsync(cancellationToken);

            if (createSnapshot)
                await CreateSnapshotAsync<TAggregateRoot>(domainEvents[0].StreamId, latestEventNumber, cancellationToken);
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

        private void Versioning(string aggregateName, string aggregateId, int version, IClientSessionHandle session, CancellationToken cancellationToken = default)
            => GetCollection<AggregateVersionModel>(aggregateName)
                .InsertOne(session, new AggregateVersionModel(aggregateId, version), cancellationToken: cancellationToken);

        private async Task CreateSnapshotAsync<TAggregateRoot>(Guid streamId, int eventNumber, CancellationToken cancellationToken = default)
            where TAggregateRoot : IAgregateRoot, new()
        {
            var aggregate = await GetAggregateAsync<TAggregateRoot>(streamId, cancellationToken);
            var snapshot = new AggregateSnapshot<TAggregateRoot>(eventNumber, aggregate, streamId);
            var snapshotCollection = GetCollection<AggregateSnapshot<TAggregateRoot>>(GetSnapshotName<TAggregateRoot>());

            await snapshotCollection.ReplaceOneAsync(a => a.StreamId == streamId, snapshot, new ReplaceOptions
            {
                IsUpsert = true
            }, cancellationToken);
        }

        public async Task<TAggregateRoot> GetAggregateAsync<TAggregateRoot>(Guid streamId, CancellationToken cancellationToken = default(CancellationToken))
            where TAggregateRoot : IAgregateRoot, new()
        {
            List<DomainEvent> events;
            var latestSnapshot = GetCollection<AggregateSnapshot<TAggregateRoot>>(GetSnapshotName<TAggregateRoot>())
                    .Find(a => a.StreamId == streamId)
                    .FirstOrDefault(cancellationToken);

            var snapshotExists = latestSnapshot is not null;
            if (!snapshotExists)
            {
                events = await GetEventsAsync<TAggregateRoot>(streamId, cancellationToken: cancellationToken);
            }
            else
            {
                events = await GetEventsAsync<TAggregateRoot>(streamId, latestSnapshot.EventNumber, cancellationToken: cancellationToken);
            }

            if (!snapshotExists && events.Count == 0)
                return default;

            var aggregate = latestSnapshot is null ? new TAggregateRoot() : latestSnapshot.Aggregate;
            aggregate.Load(events);

            return aggregate;
        }

        private IMongoCollection<T> GetCollection<T>(string streamName)
            => _database.GetCollection<T>($"{streamName}");

        private Task<List<DomainEvent>> GetEventsAsync<T>(Guid streamId, int? eventNumber = null, CancellationToken cancellationToken = default(CancellationToken))
            where T: IAgregateRoot, new()
        {
            if(!eventNumber.HasValue)
                return GetCollection<DomainEvent>(GetStreamName<T>()).Find(s => s.StreamId == streamId).ToListAsync(cancellationToken);

            return GetCollection<DomainEvent>(GetStreamName<T>()).Find(s => s.StreamId == streamId && s.EventNumber > eventNumber).ToListAsync(cancellationToken);
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
