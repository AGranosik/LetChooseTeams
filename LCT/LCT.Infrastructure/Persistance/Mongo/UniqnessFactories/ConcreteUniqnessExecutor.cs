using LCT.Domain.Common.BaseTypes;
using LCT.Domain.Common.Interfaces;
using MongoDB.Driver;

namespace LCT.Infrastructure.Persistance.Mongo.UniqnessFactories
{
    public interface IConcreteUniqnessExecutor<TEvent>
        where TEvent : IUniqness
    {
        Task ExcecuteAsync(IClientSessionHandle session, UniqnessModel domainEvent, string collectionName);
    }
    public class ConcreteUniqnessExecutor<TEvent> : IConcreteUniqnessExecutor<TEvent>
        where TEvent : IUniqness
    {
        private readonly IMongoDatabase _database;
        public ConcreteUniqnessExecutor(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task ExcecuteAsync(IClientSessionHandle session, UniqnessModel domainEvent, string collectionName)
        {
            var collection = _database.GetCollection<UniqnessModel>(collectionName);
            await collection.DeleteManyAsync(session, u => u.StreamId == domainEvent.StreamId);
            await collection.InsertOneAsync(session, domainEvent);
        }
    }
}
