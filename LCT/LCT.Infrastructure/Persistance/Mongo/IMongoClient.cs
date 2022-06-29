using LCT.Core.Entites.Tournaments.Events;
using MongoDB.Driver;

namespace LCT.Infrastructure.Persistance.Mongo
{
    public interface IMongoPersistanceClient
    {
        IMongoCollection<BaseEvent> TournamentStream { get; }
    }

    public class MongoPersistanceClient : IMongoPersistanceClient
    {
        private readonly MongoClient _mongoClient;
        private readonly string _dbName;
        public MongoPersistanceClient(MongoClient mongoClient, MongoSettings mongoSettings)
        {
            _mongoClient = mongoClient;
            _dbName = mongoSettings.DatabaseName;
        }

        public IMongoCollection<BaseEvent> TournamentStream
            => _mongoClient.GetDatabase(_dbName).GetCollection<BaseEvent>("Tournament");
    }
}
