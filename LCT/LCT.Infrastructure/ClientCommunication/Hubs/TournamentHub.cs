using LCT.Application.Teams.Events.Actions;
using LCT.Infrastructure.MessageBrokers.Interfaces;
using LCT.Infrastructure.MessageBrokers.Models;
using MediatR;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using Polly;
using Polly.Fallback;
using Polly.Retry;
using Serilog;

namespace LCT.Infrastructure.ClientCommunication.Hubs
{
    public class TournamentHub : Hub
    {
        private readonly IMediator _mediator;
        private readonly IMessageBroker _messageBroker;
        private readonly AsyncRetryPolicy _retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                );
        private readonly AsyncFallbackPolicy _fallbackPoliocy = Policy.Handle<Exception>()
            .FallbackAsync(async ct =>
            {
                Log.Error("Error during trying to establish connection to Redis.");
                await Task.FromResult(true);
            });
        public TournamentHub(IMediator mediator, IMessageBroker messageBroker)
        {
            _mediator = mediator;
            _messageBroker = messageBroker;
        }
        public async Task TeamClicked(TeamClickedAction action)
        {
            await _mediator.Publish(action);
        }

        public override async Task OnConnectedAsync()
        {
            //ok polaczenie nawiazane, ale jak zostanie przywrocone w trakcie to nie ma subscribe...
            // moze sprobowac sie polaczyc, gdy brakuje polaczenia, ale jak?
            var policy = Policy.WrapAsync(_fallbackPoliocy, _retryPolicy);
            policy.ExecuteAsync(() => _messageBroker.SubscribeAsync(GetMessageBrokerConnection()));
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            _messageBroker.UnsubscribeAsync(GetMessageBrokerConnection());
            await base.OnDisconnectedAsync(ex);
        }

        private string GetTournamentId()
        {
            IHttpContextFeature hcf = (IHttpContextFeature)this.Context.Features[typeof(IHttpContextFeature)];
            return hcf.HttpContext.Request.Query["tournamentId"];
        }

        private MessageBrokerConnection GetMessageBrokerConnection()
            => new(GetTournamentId(), Context.ConnectionId);

    }
}
