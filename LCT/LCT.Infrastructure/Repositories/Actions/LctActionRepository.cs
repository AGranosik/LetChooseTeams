using LCT.Infrastructure.Events;
using LCT.Infrastructure.Persistance.Mongo;

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
        {
            var collection = _persistanceClient.GetCollection<LctAction>(typeof(TLctAction).Name);
            await collection.InsertOneAsync(action);
        }
    }
}
