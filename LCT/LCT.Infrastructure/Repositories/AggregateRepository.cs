using LCT.Core.Shared.BaseTypes;
using LCT.Core.Shared.Exceptions;
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
                throw new EntityDoesNotExist(typeof(TAggregateRoot).Name);
            var aggregate = new TAggregateRoot();
            aggregate.Load(1, result);
            return aggregate;
        }

        public async Task SaveAsync(TAggregateRoot model)
        {
            var changes = model.GetChanges();
            var numberOfChanges = changes.Length;
            if (numberOfChanges > 1)
                await _client.GetStream(typeof(TAggregateRoot).Name).InsertManyAsync(changes);
            else if(numberOfChanges == 1)
                await _client.GetStream(typeof(TAggregateRoot).Name).InsertOneAsync(model.GetChanges().First());
        }
    }
}
