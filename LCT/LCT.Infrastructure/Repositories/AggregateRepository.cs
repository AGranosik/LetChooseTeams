using LCT.Core.Entites;
using MongoDB.Driver;

namespace LCT.Infrastructure.Repositories
{
    public class AggregateRepository<T> : IRepository<T>
        where T : Aggregate
    {
        private readonly IMongoClient _client;
        public AggregateRepository(IMongoClient client)
        {
            _client = client;
        }
        public Task<T> Load(Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task Save(T model)
        {
            throw new NotImplementedException();
        }
    }
}
