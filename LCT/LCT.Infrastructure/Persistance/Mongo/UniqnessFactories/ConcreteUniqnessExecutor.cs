using LCT.Domain.Common.BaseTypes;
using LCT.Domain.Common.Interfaces;
using MongoDB.Driver;

namespace LCT.Infrastructure.Persistance.Mongo.UniqnessFactories
{
    public interface IConcreteUniqnessExecutor<TEvent>
        where TEvent : IUniqness
    {
        Task ExcecuteAsync(IMongoCollection<IUniqness> collection, IClientSessionHandle session, IUniqness domainEvent);
    }
    public class ConcreteUniqnessExecutor<TEvent> : IConcreteUniqnessExecutor<TEvent>
        where TEvent : IUniqness
    {
        public async Task ExcecuteAsync(IMongoCollection<IUniqness> collection, IClientSessionHandle session, IUniqness domainEvent)
        {
            await collection.InsertOneAsync(session, domainEvent);
        }
    }
}
