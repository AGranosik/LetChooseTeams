using LCT.Core.Shared.BaseTypes;
using LCT.Core.Shared.Exceptions;
using LCT.Infrastructure.Persistance.Mongo;
using MediatR;
using MongoDB.Driver;
using Serilog;

namespace LCT.Infrastructure.Repositories
{
    internal class AggregateRepository<TAggregateRoot> : IAggregateRepository<TAggregateRoot>
        where TAggregateRoot : IAgregateRoot, new()
    {
        private readonly IPersistanceClient _client;
        private readonly IMediator _mediator;
        public AggregateRepository(IPersistanceClient client, IMediator mediator)
        {
            _client = client;
            _mediator = mediator;
        }
        public async Task<TAggregateRoot> LoadAsync(Guid Id)
        {
            var t = await _client.GetCollection<DomainEvent>(typeof(TAggregateRoot).Name).FindAsync(ts => ts.StreamId == Id);
            var result = await t.ToListAsync();
            if (result.Count == 0)
                throw new EntityDoesNotExist(typeof(TAggregateRoot).Name);
            var aggregate = new TAggregateRoot();
            aggregate.Load(result);
            return aggregate;
        }

        public async Task SaveAsync(TAggregateRoot model)
        {
            var events = model.GetChanges();
            await SaveToStreamAsync(events);
            try
            {
                await PublishEventsAsync(events);
            }
            catch(Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
        }

        private async Task PublishEventsAsync(DomainEvent[] events)
        {
            foreach (var @event in events)
                await _mediator.Publish(@event, CancellationToken.None);
        }

        private async Task SaveToStreamAsync(DomainEvent[] events)
        {
            var numberOfChanges = events.Length;
            if (numberOfChanges > 1)
                await _client.GetCollection<DomainEvent>(typeof(TAggregateRoot).Name).InsertManyAsync(events);
            else if (numberOfChanges == 1)
                await _client.GetCollection<DomainEvent>(typeof(TAggregateRoot).Name).InsertOneAsync(events.First());
        }
    }
}
