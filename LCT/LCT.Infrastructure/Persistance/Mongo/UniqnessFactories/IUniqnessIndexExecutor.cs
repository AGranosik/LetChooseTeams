using LCT.Domain.Common.BaseTypes;
using LCT.Domain.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LCT.Infrastructure.Persistance.Mongo.UniqnessFactories
{
    internal interface IUniqnessIndexExecutor //register prop name & event?
    {
        Task ExcecuteAsync(IMongoDatabase database, IClientSessionHandle session, DomainEvent domainEvent);
        IUniqnessIndexExecutor RegisterUniqnessForEvent<TPropertyName, TEvent>(string aggregateName, IMongoDatabase database)
            where TEvent : IUniqness;
    }

    internal class UniqnessIndexExecutor : IUniqnessIndexExecutor
    {
        private readonly CreateIndexOptions _uniqueIndexOptions = new CreateIndexOptions { Unique = true };
        private Dictionary<string, IMongoCollection<IUniqness>> _uniqnessCollections = new Dictionary<string, IMongoCollection<IUniqness>>();
        public async Task ExcecuteAsync(IMongoDatabase database, IClientSessionHandle session, DomainEvent domainEvent)
        {
            var eventName = typeof(DomainEvent).Name;
            //var collection = _uniqnessCollections
        }

        public IUniqnessIndexExecutor RegisterUniqnessForEvent<TPropertyName, TEvent>(string aggregateName, IMongoDatabase database)
            where TEvent : IUniqness
        {
            var indexModel = new CreateIndexModel<TEvent>(new BsonDocument("UniqueValue", 1), _uniqueIndexOptions);
            var eventName = typeof(TEvent).Name;
            var collection = database.GetCollection<TEvent>($"{aggregateName}_{eventName}_index");
            collection
                .Indexes.CreateOne(indexModel);

            _uniqnessCollections.Add(eventName, (IMongoCollection<IUniqness>)collection); // store as func?

            return this;
        }
    }


}
