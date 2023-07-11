using LCT.Domain.Common.BaseTypes;
using LCT.Domain.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace LCT.Infrastructure.Persistance.EventsStorage.UniqnessFactories
{
    public interface IUniqnessIndexExecutor
    {
        Task ExcecuteAsync(IClientSessionHandle session, DomainEvent domainEvent);
        IUniqnessIndexExecutor RegisterUniqnessForEvent<TEvent>(string aggregateName, IMongoDatabase database)
            where TEvent : IUniqness;
    }

    public class UniqnessIndexExecutor : IUniqnessIndexExecutor
    {
        private readonly CreateIndexOptions _uniqueIndexOptions = new CreateIndexOptions { Unique = true };
        private Dictionary<string, Func<IClientSessionHandle, UniqnessModel, Task>> _uniqnessCollections = new();
        public async Task ExcecuteAsync(IClientSessionHandle session, DomainEvent domainEvent)
        {
            var eventName = domainEvent.GetType().Name;
            var func = _uniqnessCollections[eventName];
            var uniqueValue = ((IUniqness)domainEvent).UniqueValue;

            await func.Invoke(session, new UniqnessModel
            {
                StreamId = domainEvent.StreamId.ToString(),
                UniqueValue = uniqueValue
            });
        }

        public IUniqnessIndexExecutor RegisterUniqnessForEvent<TEvent>(string aggregateName, IMongoDatabase database)
            where TEvent : IUniqness
        {
            var indexModel = new CreateIndexModel<UniqnessModel>(Builders<UniqnessModel>.IndexKeys.Ascending(u => u.UniqueValue), _uniqueIndexOptions);
            var eventName = typeof(TEvent).Name;
            var collectionName = $"{aggregateName}_{eventName}_index";
            var collection = database.GetCollection<UniqnessModel>(collectionName);
            collection
                .Indexes.CreateOne(indexModel);

            Func<IClientSessionHandle, UniqnessModel, Task> func =
                (session, @event) => new ConcreteUniqnessExecutor<TEvent>(database).ExcecuteAsync(session, @event, collectionName);
            _uniqnessCollections.Add(eventName, func);
            BsonClassMap.RegisterClassMap<TEvent>();
            return this;
        }
    }


    public class UniqnessModel
    {
        public string StreamId { get; set; }
        public string UniqueValue { get; set; }
    }


}
