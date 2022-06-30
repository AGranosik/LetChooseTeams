using LCT.Core.Entites.Tournaments.Events;
using MongoDB.Driver;

namespace LCT.Infrastructure.Persistance.Mongo
{
    public interface IMongoPersistanceClient
    {
        IMongoCollection<object> TournamentStream { get; }
    }

    public class MongoPersistanceClient : IMongoPersistanceClient
    {
        private readonly IMongoClient _mongoClient;
        private readonly string _dbName;
        public MongoPersistanceClient(IMongoClient mongoClient, MongoSettings mongoSettings)
        {
            _mongoClient = mongoClient;
            _dbName = mongoSettings.DatabaseName;
        }

        public IMongoCollection<object> TournamentStream
            => _mongoClient.GetDatabase(_dbName).GetCollection<object>("TournamentEvents");
    }
}
