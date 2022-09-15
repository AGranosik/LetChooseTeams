using LCT.Application.Tournaments.Actions.TeamChooseAction;
using LCT.Infrastructure.Repositories.Actions;
using Microsoft.AspNetCore.SignalR;

namespace LCT.Application.Tournaments.Actions
{
    public class ActionHub : Hub
    {
        public ActionHub()
        {

        }
        public async Task Action(string message)
        {
        }

        public async Task Action2(TeamSelectedAction message)
        {
            await Clients.All.SendAsync("Test", message);
        }
    }
}
