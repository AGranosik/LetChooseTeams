using LCT.Application.Teams.Events.Actions;
using LCT.Infrastructure.MessageBrokers.Interfaces;
using LCT.Infrastructure.MessageBrokers.Models;
using MediatR;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;

namespace LCT.Infrastructure.ClientCommunication.Hubs
{
    public class TournamentHub : Hub
    {
        private readonly IMediator _mediator;
        private readonly IMessageBroker _messageBroker;
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
            await _messageBroker.SubscribeAsync(GetMessageBrokerConnection());
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await _messageBroker.UnsubscribeAsync(GetMessageBrokerConnection());
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
