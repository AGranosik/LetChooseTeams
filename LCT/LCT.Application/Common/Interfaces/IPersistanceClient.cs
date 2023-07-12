using System.Linq.Expressions;
using LCT.Domain.Common.BaseTypes;

namespace LCT.Application.Common.Interfaces
{
    public interface IPersistanceClient
    {
        Task SaveEventAsync<TAggregate>(DomainEvent[] domainEvent, int version = 0)
            where TAggregate : IAgregateRoot, new();

        Task<TAggregateRoot> GetAggregateAsync<TAggregateRoot>(Guid streamId)
            where TAggregateRoot : IAgregateRoot, new();
    }


}
