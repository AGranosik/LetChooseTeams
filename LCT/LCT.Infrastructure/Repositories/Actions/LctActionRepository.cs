using LCT.Application.Common.Events;
using LCT.Application.Common.Interfaces;

namespace LCT.Infrastructure.Repositories.Actions
{
    public class LctActionRepository<TLctAction> : ILctActionRepository<TLctAction>
        where TLctAction : LctAction
    {
        private readonly IPersistanceClient _persistanceClient;
        public LctActionRepository(IPersistanceClient persistanceClient)
        {
            _persistanceClient = persistanceClient;
        }

        public async Task SaveAsync(TLctAction action)
            => await _persistanceClient.SaveActionAsync(action);
    }
}
