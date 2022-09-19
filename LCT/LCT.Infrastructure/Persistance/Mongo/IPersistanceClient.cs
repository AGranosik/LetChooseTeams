using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams;
using LCT.Infrastructure.Persistance.Mongo.UniqnessModels;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LCT.Infrastructure.Persistance.Mongo
{
    //check if index is on stream id for faster search
    public interface IPersistanceClient
    {
        IMongoCollection<T> GetCollection<T>(string streamName);
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

        public IMongoCollection<T> GetCollection<T>(string streamName) //more generic if we change from mongo into redis there will be too much to change in return types
            => _mongoClient.GetDatabase(_dbName).GetCollection<T>($"{streamName}Stream");

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

            var indexModel2 = new CreateIndexModel<TeamSelectionUniqnessModel>(
                teamSelectionIndex,
                indexOptions);

            mongoDatabase.GetCollection<TeamSelectionUniqnessModel>("Tournament_SelectedTeams_index")
                .Indexes.CreateOne(indexModel2);
        }
    }
}
