using LCT.Application.Common.Events;
using LCT.Application.Common.Interfaces;
using LCT.Application.Common.UniqnessModels;
using LCT.Core.Shared.BaseTypes;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LCT.Infrastructure.Persistance.Mongo
{
    public class MongoPersistanceClient : IPersistanceClient
    {
        private readonly IMongoClient _mongoClient;
        private readonly string _dbName;
        public MongoPersistanceClient(IMongoClient mongoClient, MongoSettings mongoSettings)
        {
            _mongoClient = mongoClient;
            _dbName = mongoSettings.DatabaseName;
            Configure();
        }

        public async Task<bool> CheckUniqness<T>(string entity, string fieldName, T value)
        {
            var collection = _mongoClient.GetDatabase(_dbName).GetCollection<T>($"{entity}_{fieldName}_index");
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

        public void Configure()
        {
            var mongoDatabase = _mongoClient.GetDatabase(_dbName);
            var createIndexOptions = new CreateIndexOptions { Unique = true };

            ConfigureTournamentNameUniqnes(mongoDatabase, createIndexOptions);
            ConfigureTournamentTeamSelection(mongoDatabase, createIndexOptions);
            ConfigureDomainEventIndex(mongoDatabase);
        }

        public async Task SaveEventAsync<TAggregateRoot>(DomainEvent domainEvent)
            where TAggregateRoot : IAgregateRoot
            => await GetCollection<DomainEvent>($"{typeof(TAggregateRoot).Name}Stream").InsertOneAsync(domainEvent);

        public Task SaveActionAsync<T>(T action) where T : LctAction
            => GetCollection<T>($"{typeof(T).Name}").InsertOneAsync(action);

        private IMongoCollection<T> GetCollection<T>(string streamName)
            => _mongoClient.GetDatabase(_dbName).GetCollection<T>($"{streamName}");

        public async Task SaveEventsAsync<TAggregateRoot>(List<DomainEvent> domainEvents)
            where TAggregateRoot : IAgregateRoot
            => await GetCollection<DomainEvent>($"{typeof(TAggregateRoot).Name}Stream").InsertManyAsync(domainEvents);

        public async Task<List<DomainEvent>> GetEventsAsync<T>(Guid streamId)
            where T: IAgregateRoot
        {
            var cursorAsync = await GetCollection<DomainEvent>($"{typeof(T).Name}Stream").FindAsync(s => s.StreamId == streamId);
            return await cursorAsync.ToListAsync();
        }
        private void ConfigureDomainEventIndex(IMongoDatabase mongoDatabase)
        {
            var indexModelBuilder = Builders<DomainEvent>.IndexKeys
                .Text(e => e.StreamId);

            var indexModel = new CreateIndexModel<DomainEvent>(indexModelBuilder);

            mongoDatabase.GetCollection<DomainEvent>($"{nameof(Tournament)}Stream")
                .Indexes.CreateOne(indexModel);
        }

        private void ConfigureTournamentNameUniqnes(IMongoDatabase mongoDatabase, CreateIndexOptions indexOptions)
        {
            var indexModel = new CreateIndexModel<TournamentName>(new BsonDocument("Value", 1), indexOptions);
            mongoDatabase.GetCollection<TournamentName>("Tournament_TournamentName_index")
                .Indexes.CreateOne(indexModel);
        }

        private void ConfigureTournamentTeamSelection(IMongoDatabase mongoDatabase, CreateIndexOptions indexOptions)
        {
            var teamSelectionIndex = Builders<TeamSelectionUniqnessModel>.IndexKeys
                .Ascending(l => l.Team)
                .Ascending(l => l.TournamentId);

            var indexModel = new CreateIndexModel<TeamSelectionUniqnessModel>(
                teamSelectionIndex,
                indexOptions);

            mongoDatabase.GetCollection<TeamSelectionUniqnessModel>("Tournament_SelectedTeams_index")
                .Indexes.CreateOne(indexModel);
        }


    }
}
