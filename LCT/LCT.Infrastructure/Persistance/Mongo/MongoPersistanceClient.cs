using LCT.Application.Common.Events;
using LCT.Application.Common.Interfaces;
using LCT.Application.Common.UniqnessModels;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Events;
using LCT.Domain.Common.BaseTypes;
using LCT.Domain.Common.Interfaces;
using LCT.Infrastructure.Persistance.Mongo.UniqnessFactories;
using LCT.Infrastructure.Persistance.Mongo.UniqnessFactories.Models;
using MongoDB.Driver;

namespace LCT.Infrastructure.Persistance.Mongo
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
            where TAggregateRoot : IAgregateRoot
        {
            using var session = await _mongoClient.StartSessionAsync();
            session.StartTransaction();
            bool isVersionable = false;
            bool createSnapshot = false;
            var aggregateId = domainEvents[0].StreamId.ToString();

            foreach(var domainEvent in domainEvents)
            {
                if(domainEvent.EventNumber % 5 == 0)
                    createSnapshot = true;

                if(domainEvent is IVersionable)
                    isVersionable = true;

                if(domainEvent is IUniqness)
                {
                    await _uniqnessExecutor.ExcecuteAsync(session, domainEvent);
                }
                await GetCollection<DomainEvent>(GetStreamName<TAggregateRoot>()).InsertOneAsync(session, domainEvent);
            }

            if (isVersionable)
                await Versioning(GetVersionIndex<TAggregateRoot>(), aggregateId, version, session);

            if (createSnapshot)
                await CreateSnapshot<TAggregateRoot>(aggregateId);

            await session.CommitTransactionAsync();
        }

        public Task SaveActionAsync<T>(T action) where T : LctAction
            => GetCollection<T>($"{typeof(T).Name}").InsertOneAsync(action);

        public static string GetStreamName<TAggregate>()
            where TAggregate : IAgregateRoot
            => $"{typeof(TAggregate).Name}_Stream";

        public static string GetVersionIndex<TAggregate>()
            where TAggregate : IAgregateRoot
            => $"{typeof(TAggregate).Name}_Version_index";

        private async Task Versioning(string aggregateName, string aggregateId, int version, IClientSessionHandle session)
        {
            await GetCollection<AggregateVersionModel>(aggregateName)
                .InsertOneAsync(session, new AggregateVersionModel(aggregateId, version));
        }

        private async Task CreateSnapshot<TAggregateRoot>(string aggregateId, IClientSessionHandle session)
            where TAggregateRoot : IAgregateRoot
        {

        }

        private IMongoCollection<T> GetCollection<T>(string streamName)
            => _database.GetCollection<T>($"{streamName}");

        public async Task<List<DomainEvent>> GetEventsAsync<T>(Guid streamId)
            where T: IAgregateRoot
        {
            var cursorAsync = await GetCollection<DomainEvent>(GetStreamName<T>()).FindAsync(s => s.StreamId == streamId);
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

        public async Task<List<T>> GetActionsAsync<T, TKey>(TKey aggregateId)
             where T : LctAction<TKey>
        {
            var cursorAsync = await GetCollection<T>($"{typeof(T).Name}").FindAsync(t => t.GroupKey.Equals(aggregateId));
            return await cursorAsync.ToListAsync();
        }
    }
}
