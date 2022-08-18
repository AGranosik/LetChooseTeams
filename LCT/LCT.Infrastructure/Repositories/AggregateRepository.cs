using LCT.Core.Shared.BaseTypes;
using LCT.Infrastructure.Persistance.Mongo;
using MongoDB.Driver;

namespace LCT.Infrastructure.Repositories
{
    public class AggregateRepository<T, TKey> : IRepository<T, TKey>
        where T : Aggregate<TKey>, new ()
        where TKey : ValueType<TKey>
    {
        private readonly IPersistanceClient _client;
        public AggregateRepository(IPersistanceClient client)
        {
            _client = client;
        }
        public async Task<T> LoadAsync(Guid Id)
        {
            var t = await _client.GetStream(typeof(T).Name).FindAsync(ts => ts.StreamId == Id);
            var result = t.ToList();
            if (result.Count == 0)
                return null;
            var aggregate = new T();
            aggregate.Load(1, result);
            return aggregate;
        }

        public async Task SaveAsync(T model)
        {
            await _client.GetStream(typeof(T).Name).InsertOneAsync(model.GetChanges().Last());
        }
    }
}
