using LCT.Application.Common.Events;
using LCT.Application.Common.Interfaces;
using LCT.Infrastructure.Persistance.ActionsStorage;

namespace LCT.Infrastructure.Repositories.Actions
{
    public class LctActionRepository<TLctAction, TKey> : ILctActionRepository<TLctAction, TKey>
        where TLctAction : LctAction<TKey>
    {
        private readonly IActionStorageClient _actionPersistanceClient;
        public LctActionRepository(IActionStorageClient actionPersistanceClient)
        {
            _actionPersistanceClient = actionPersistanceClient;
        }

        public async Task<List<TLctAction>> GetByGroupIdAsync(TKey aggregateId, CancellationToken cancellationToken = default)
            => await _actionPersistanceClient.GetActionsAsync<TLctAction, TKey>(aggregateId);

        public async Task SaveAsync(TLctAction action, CancellationToken cancellationToken = default)
            => await _actionPersistanceClient.SaveActionAsync(action);
    }
}
