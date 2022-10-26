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

        Task SaveEventAsync<TAggregate>(DomainEvent domainEvent, string aggregateId = "", int version = 0)
            where TAggregate : IAgregateRoot;

        Task<List<DomainEvent>> GetEventsAsync<T>(Guid streamId)
            where T: IAgregateRoot;

        Task<bool> CheckUniqness<T>(string entity, string fieldName, T value);
        void Configure();
    }


}
