using LCT.Core.Entites.Tournaments.Events;
using MongoDB.Driver;

namespace LCT.Infrastructure.Persistance.Mongo
{
    public interface IPersistanceClient
    {
        IMongoCollection<BaseEvent> GetStream(string streamName);
        Task<bool> CheckUniqness<T>(string entity, string fieldName, T value);
    }

    public class MongoPersistanceClient : IPersistanceClient
    {
        private readonly IMongoClient _mongoClient;
        private readonly string _dbName;
        public MongoPersistanceClient(IMongoClient mongoClient, MongoSettings mongoSettings)
        {
            _mongoClient = mongoClient;
            _dbName = mongoSettings.DatabaseName;
        }

        public IMongoCollection<BaseEvent> GetStream(string streamName)
            => _mongoClient.GetDatabase(_dbName).GetCollection<BaseEvent>($"{streamName}Stream");

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
    }
}
