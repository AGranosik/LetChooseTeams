using LCT.Application.Teams.Events.Actions;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace LCT.Application.Tournaments.Hubs
{
    //should be infrastructure
    public class TournamentHub : Hub
    {
        private readonly IMediator _mediator;
        public TournamentHub(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task TeamClicked(TeamClickedAction action)
        {
            Log.Warning("wybrano: " + Context.ConnectionId);
            await _mediator.Publish(action);
        }

        public override async Task OnConnectedAsync()
        {
            Log.Warning("connected: " + Context.ConnectionId);
            Log.Warning("pod: " + Environment.MachineName);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            Log.Warning("disconnected: " + Context.ConnectionId);
            await base.OnDisconnectedAsync(ex);
        }

    }
}
