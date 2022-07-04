using LCT.Core.Entites;
using LCT.Infrastructure.Persistance.Mongo;
using MongoDB.Driver;

namespace LCT.Infrastructure.Repositories
{
    public class AggregateRepository<T> : IRepository<T>
        where T : Aggregate
    {
        private readonly IMongoPersistanceClient _client;
        public AggregateRepository(IMongoPersistanceClient client)
        {
            _client = client;
        }
        public async Task<T> Load(Guid Id)
        {
            var t = await _client.TournamentStream.FindAsync(ts => true);
            var result = t.ToList();
            return null;
        }

        public async Task Save(T model)
        {
            await _client.TournamentStream.InsertOneAsync(model.GetChanges().Last());
        }
    }
}
