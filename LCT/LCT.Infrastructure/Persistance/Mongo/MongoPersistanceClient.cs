using LCT.Application.Common.Events;
using LCT.Application.Common.Interfaces;
using LCT.Application.Common.UniqnessModels;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Events;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams;
using LCT.Domain.Common.BaseTypes;
using LCT.Domain.Common.Interfaces;
using LCT.Infrastructure.Persistance.Mongo.UniqnessFactories;
using MongoDB.Bson;
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

        public async Task<bool> CheckUniqness<T>(string entity, string fieldName, T value)
        {
            //create index here? instead of splitting it up in different files
            var collection = _database.GetCollection<T>($"{entity}_{fieldName}_index");
            try
            {
                await collection.InsertOneAsync(value);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task SaveEventAsync<TAggregateRoot>(DomainEvent[] domainEvents, string aggregateId = "", int version = 0) // more generic?
            where TAggregateRoot : IAgregateRoot
        {
            var aggregateName = typeof(TAggregateRoot).Name;
            using var session = await _mongoClient.StartSessionAsync();
            session.StartTransaction();

            foreach(var domainEvent in domainEvents)
            {
                if(domainEvent is IVersionable)
                {
                    await Versioning($"{aggregateName}_Version_index", aggregateId, version, session); //multiple versioning events
                }

                if(domainEvent is IUniqness)
                {
                    await _uniqnessExecutor.ExcecuteAsync(_database, session, domainEvent);
                }
                await GetCollection<DomainEvent>($"{aggregateName}Stream").InsertOneAsync(session, domainEvent);
            }
            await session.CommitTransactionAsync();
        }

        public Task SaveActionAsync<T>(T action) where T : LctAction
            => GetCollection<T>($"{typeof(T).Name}").InsertOneAsync(action);

        private async Task Versioning(string aggregateName, string aggregateId, int version, IClientSessionHandle session)
        {
            await GetCollection<AggregateVersionModel>(aggregateName)
                .InsertOneAsync(session, new AggregateVersionModel(aggregateId, version));
        }

        private IMongoCollection<T> GetCollection<T>(string streamName)
            => _database.GetCollection<T>($"{streamName}");

        public async Task<List<DomainEvent>> GetEventsAsync<T>(Guid streamId)
            where T: IAgregateRoot
        {
            var cursorAsync = await GetCollection<DomainEvent>($"{typeof(T).Name}Stream").FindAsync(s => s.StreamId == streamId);
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
        {
            var teamSelectionIndex = Builders<AggregateVersionModel>.IndexKeys
                .Ascending(l => l.AggregateId)
                .Ascending(l => l.Version);

            var indexModel = new CreateIndexModel<AggregateVersionModel>(
                teamSelectionIndex,
                indexOptions);

            _database.GetCollection<AggregateVersionModel>($"{typeof(TAggregate).Name}_Version_index") //fucntion for that names
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
