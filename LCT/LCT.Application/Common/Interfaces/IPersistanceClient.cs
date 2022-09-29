using LCT.Application.Common.Events;
using LCT.Domain.Common.BaseTypes;

namespace LCT.Application.Common.Interfaces
{
    public interface IPersistanceClient
    {
        Task SaveActionAsync<T>(T action) //move it to different interface
            where T : LctAction;

        Task SaveEventAsync<TAggregate>(DomainEvent domainEvent)
            where TAggregate : IAgregateRoot;

        Task SaveEventsAsync<TAggregate>(List<DomainEvent> domainEvents)
            where TAggregate : IAgregateRoot;

        Task<List<DomainEvent>> GetEventsAsync<T>(Guid streamId)
            where T: IAgregateRoot;

        Task<bool> CheckUniqness<T>(string entity, string fieldName, T value);
        void Configure();
    }


}
