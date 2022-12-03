using LCT.Application.Common.Events;
using LCT.Domain.Common.BaseTypes;

namespace LCT.Application.Common.Interfaces
{
    public interface IPersistanceClient
    {
        Task<List<T>> GetActionsAsync<T, TKey>(TKey aggregateId)
             where T : LctAction<TKey>;
        Task SaveActionAsync<T>(T action)
            where T : LctAction;

        Task SaveEventAsync<TAggregate>(DomainEvent[] domainEvent, int version = 0)
            where TAggregate : IAgregateRoot, new();

        Task<List<DomainEvent>> GetEventsAsync<T>(Guid streamId)
            where T: IAgregateRoot, new();

        Task<TAggregateRoot> GetAggregate<TAggregateRoot>(Guid streamId)
            where TAggregateRoot : IAgregateRoot, new();
    }


}
