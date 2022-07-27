﻿using LCT.Core.Entites;
using LCT.Infrastructure.Persistance.Mongo;
using MongoDB.Driver;

namespace LCT.Infrastructure.Repositories
{
    public class AggregateRepository<T> : IRepository<T>
        where T : Aggregate, new ()
    {
        private readonly IMongoPersistanceClient _client;
        public AggregateRepository(IMongoPersistanceClient client)
        {
            _client = client;
        }
        public async Task<T> Load(Guid Id)
        {
            var t = await _client.GetStream(nameof(T)).FindAsync(ts => ts.StreamId == Id);
            var result = t.ToList();
            var aggregate = new T();
            aggregate.Load(1, result);
            return aggregate;
        }

        public async Task Save(T model)
        {
            await _client.GetStream(nameof(T)).InsertOneAsync(model.GetChanges().Last());
        }
    }
}
