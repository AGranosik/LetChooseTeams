using LCT.Core.Shared.BaseTypes;
using LCT.Infrastructure.Persistance.Mongo;
using MongoDB.Driver;

namespace LCT.Infrastructure.Repositories
{
    public class AggregateRepository<TAggregateRoot> : IRepository<TAggregateRoot>
        where TAggregateRoot : IAgregateRoot, new()
    {
        private readonly IPersistanceClient _client;
        public AggregateRepository(IPersistanceClient client)
        {
            _client = client;
        }
        public async Task<TAggregateRoot> LoadAsync(Guid Id)
        {
            var t = await _client.GetStream(typeof(TAggregateRoot).Name).FindAsync(ts => ts.StreamId == Id);
            var result = t.ToList();
            if (result.Count == 0)
                return default(TAggregateRoot);
            var aggregate = new TAggregateRoot();
            aggregate.Load(1, result);
            return aggregate;
        }

        public async Task SaveAsync(TAggregateRoot model)
        {
            await _client.GetStream(typeof(TAggregateRoot).Name).InsertOneAsync(model.GetChanges().Last());
        }
    }
}
