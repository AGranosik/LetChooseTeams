using LCT.Domain.Common.BaseTypes;

namespace LCT.Application.Common.Interfaces
{
    public interface IPersistanceClient
    {
        Task SaveEventAsync<TAggregate>(DomainEvent[] domainEvent, int version = 0, CancellationToken cancellationToken = default(CancellationToken))
            where TAggregate : IAgregateRoot, new();

        Task<TAggregateRoot> GetAggregateAsync<TAggregateRoot>(Guid streamId, CancellationToken cancellationToken = default(CancellationToken))
            where TAggregateRoot : IAgregateRoot, new();
    }


}
