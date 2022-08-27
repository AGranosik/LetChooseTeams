using LCT.Core.Shared.BaseTypes;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LCT.Infrastructure.Persistance.Mongo
{
    public interface IPersistanceClient
    {
        IMongoCollection<DomainEvent> GetStream(string streamName);
        Task<bool> CheckUniqness<T>(string entity, string fieldName, T value);
        void Configure();
    }

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

        public IMongoCollection<DomainEvent> GetStream(string streamName)
            => _mongoClient.GetDatabase(_dbName).GetCollection<DomainEvent>($"{streamName}Stream");

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
            var indexModel = new CreateIndexModel<TournamentName>(new BsonDocument("Value", 1), new CreateIndexOptions { Unique = true });
            _mongoClient.GetDatabase(_dbName).GetCollection<TournamentName>("Tournament_TournamentName_index")
                .Indexes.CreateOne(indexModel);
        }
    }
}
