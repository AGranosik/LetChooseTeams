using LCT.Application.Common.Interfaces;
using LCT.Domain.Common.BaseTypes;
using LCT.Domain.Common.Exceptions;
using LCT.Domain.Common.Interfaces;
using LCT.Infrastructure.Common.Exceptions;
using MediatR;
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
        public async Task<TAggregateRoot> LoadAsync(Guid Id, CancellationToken cancellationToken)
        {
            var aggregate = await _client.GetAggregateAsync<TAggregateRoot>(Id);
            if (aggregate is null)
                throw new EntityDoesNotExist(typeof(TAggregateRoot).Name);
            return aggregate;
        }

        public async Task SaveAsync(TAggregateRoot model, int version = 0, CancellationToken cancellationToken = default)
        {
            var events = model.GetChanges();
            await _client.SaveEventAsync<TAggregateRoot>(events, version);
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
    }
}
