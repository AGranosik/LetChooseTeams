using LCT.Application.Common.Interfaces;
using LCT.Domain.Common.BaseTypes;
using LCT.Domain.Common.Exceptions;
using LCT.Domain.Common.Interfaces;
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
            var result = await _client.GetEventsAsync<TAggregateRoot>(Id);
            if (result.Count == 0)
                throw new EntityDoesNotExist(typeof(TAggregateRoot).Name);
            var aggregate = new TAggregateRoot();
            aggregate.Load(result);
            return aggregate;
        }

        public async Task SaveAsync(TAggregateRoot model, int version = 0)
        {
            var events = model.GetChanges();
            await SaveToStreamAsync(events, model.AggregateId(), version);
            await PublishEventsAsync(events);
        }

        private async Task PublishEventsAsync(DomainEvent[] events)
        {
            foreach (var @event in events)
            {
                try
                {
                    await _mediator.Publish(@event, CancellationToken.None);
                }
                catch(Exception ex)
                {
                    Log.Error(ex, ex.Message);
                }
            }
        }

        private async Task SaveToStreamAsync(DomainEvent[] events, string aggregateId, int version = 0)
        {
            foreach(var @event in events)
                await _client.SaveEventAsync<TAggregateRoot>(@event, aggregateId, version);
        }
    }
}
