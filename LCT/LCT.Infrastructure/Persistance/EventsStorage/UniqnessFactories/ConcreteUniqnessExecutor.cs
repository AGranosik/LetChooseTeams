using LCT.Domain.Common.BaseTypes;
using LCT.Domain.Common.Interfaces;
using MongoDB.Driver;

namespace LCT.Infrastructure.Persistance.EventsStorage.UniqnessFactories
{
    public interface IConcreteUniqnessExecutor<TEvent>
        where TEvent : IUniqness
    {
        void Excecute(IClientSessionHandle session, UniqnessModel domainEvent, string collectionName);
    }
    public class ConcreteUniqnessExecutor<TEvent> : IConcreteUniqnessExecutor<TEvent>
        where TEvent : IUniqness
    {
        private readonly IMongoDatabase _database;
        public ConcreteUniqnessExecutor(IMongoDatabase database)
        {
            _database = database;
        }

        public void Excecute(IClientSessionHandle session, UniqnessModel domainEvent, string collectionName)
        {
            var collection = _database.GetCollection<UniqnessModel>(collectionName);
            collection.DeleteMany(session, u => u.StreamId == domainEvent.StreamId);
            collection.InsertOne(session, domainEvent);
        }
    }
}
