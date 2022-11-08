using LCT.Domain.Common.BaseTypes;
using LCT.Domain.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LCT.Infrastructure.Persistance.Mongo.UniqnessFactories
{
    public interface IUniqnessIndexExecutor //register prop name & event?
    {
        Task ExcecuteAsync(IMongoDatabase database, IClientSessionHandle session, DomainEvent domainEvent);
        IUniqnessIndexExecutor RegisterUniqnessForEvent<TEvent>(string aggregateName, IMongoDatabase database)
            where TEvent : IUniqness;
    }

    public class UniqnessIndexExecutor : IUniqnessIndexExecutor
    {
        private readonly CreateIndexOptions _uniqueIndexOptions = new CreateIndexOptions { Unique = true };
        private Dictionary<string, Func<IClientSessionHandle, IUniqness, Task>> _uniqnessCollections = new();
        public async Task ExcecuteAsync(IMongoDatabase database, IClientSessionHandle session, DomainEvent domainEvent)
        {
            var eventName = domainEvent.GetType().Name;
            var func = _uniqnessCollections[eventName];

            await func.Invoke(session, (IUniqness)domainEvent);
        }

        public IUniqnessIndexExecutor RegisterUniqnessForEvent<TEvent>(string aggregateName, IMongoDatabase database)
            where TEvent : IUniqness
        {
            var indexModel = new CreateIndexModel<IUniqness>(new BsonDocument("UniqueValue", 1), _uniqueIndexOptions);
            var eventName = typeof(TEvent).Name;
            var collection = database.GetCollection<IUniqness>($"{aggregateName}_{eventName}_index");
            collection
                .Indexes.CreateOne(indexModel);

            Func<IClientSessionHandle, IUniqness, Task> func =
                (session, @event) => new ConcreteUniqnessExecutor<TEvent>().ExcecuteAsync(collection, session, @event);
            _uniqnessCollections.Add(eventName, func);

            return this;
        }
    }


}
