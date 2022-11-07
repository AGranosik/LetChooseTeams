using LCT.Domain.Common.BaseTypes;
using MongoDB.Driver;

namespace LCT.Infrastructure.Persistance.Mongo.UniqnessFactories
{
    internal interface IConcreteUniqnessExecutor
    {
        Task ExcecuteAsync(IMongoDatabase database, IClientSessionHandle session, DomainEvent domainEvent);
    }
    internal class ConcreteUniqnessExecutor
    {
    }
}
