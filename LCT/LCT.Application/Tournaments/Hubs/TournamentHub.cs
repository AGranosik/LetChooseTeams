using LCT.Application.Tournaments.Actions.TeamChooseAction;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace LCT.Application.Tournaments.Hubs
{
    public class TournamentHub : Hub
    {
        private readonly IMediator _mediator;
        public TournamentHub(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task TeamClicked(TeamSelectedAction action)
            => await _mediator.Send(action);
            
    }
}
