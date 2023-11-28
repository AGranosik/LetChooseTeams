using LCT.Application.Common.Events;

namespace LCT.Application.Common.Interfaces
{
    public interface ILctActionRepository<TLctAction, TKey>
        where TLctAction : LctAction<TKey>
    {
        Task SaveAsync(TLctAction action, CancellationToken cancellationToken = default);
        Task<List<TLctAction>> GetByGroupIdAsync(TKey aggregateId, CancellationToken cancellationToken = default);
    }
}
