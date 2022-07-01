using LCT.Core.Entites;
using LCT.Infrastructure.Persistance.Mongo;

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
        public Task<T> Load(Guid Id)
        {
            throw new NotImplementedException();
        }

        public async Task Save(T model)
        {
            await _client.TournamentStream.InsertOneAsync(model.GetChanges());
        }
    }
}
