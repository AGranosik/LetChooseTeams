using LCT.Application.Common.Events;
using LCT.Application.Common.Interfaces;

namespace LCT.Infrastructure.Repositories.Actions
{
    public class LctActionRepository<TLctAction, TKey> : ILctActionRepository<TLctAction, TKey>
        where TLctAction : LctAction<TKey>
    {
        private readonly IPersistanceClient _persistanceClient; // get rid of?? Just use it there
        public LctActionRepository(IPersistanceClient persistanceClient)
        {
            _persistanceClient = persistanceClient;
        }

        public async Task<List<TLctAction>> GetByGroupIdAsync(TKey aggregateId)
            => await _persistanceClient.GetActionsAsync<TLctAction, TKey>(aggregateId);

        public async Task SaveAsync(TLctAction action)
            => await _persistanceClient.SaveActionAsync(action);
    }
}
