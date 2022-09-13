using Microsoft.AspNetCore.SignalR;

namespace LCT.Application.Tournaments.Hubs
{
    public class PlayerAssignedHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("Test", message);
        }
    }
}
