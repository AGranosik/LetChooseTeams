using LCT.Application.Common.Events;
using LCT.Infrastructure.Persistance.EventsStorage;
using MongoDB.Driver;

namespace LCT.Infrastructure.Persistance.ActionsStorage
{
    internal class MongoActionStorageClient : IActionStorageClient
    {
        private readonly IMongoClient _mongoClient;
        private readonly string _dbName;
        private readonly IMongoDatabase _database;
        public MongoActionStorageClient(IMongoClient mongoClient, MongoSettings mongoSettings)
        {
            _mongoClient = mongoClient;
            _dbName = mongoSettings.DatabaseName;
            _database = _mongoClient.GetDatabase(_dbName);
        }
        public async Task<List<T>> GetActionsAsync<T, TKey>(TKey aggregateId) where T : LctAction<TKey>
        {
            var cursorAsync = await _database.GetCollection<T>($"{typeof(T).Name}").FindAsync(t => t.GroupKey.Equals(aggregateId));
            return await cursorAsync.ToListAsync();
        }

        public async Task SaveActionAsync<T>(T action) where T : LctAction
            => await _database.GetCollection<T>($"{typeof(T).Name}").InsertOneAsync(action);
    }
}
