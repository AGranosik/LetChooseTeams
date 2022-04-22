using Microsoft.AspNetCore.SignalR;

namespace LCT.Application.Tournaments.Hubs
{
    public class TeamSelectedHub : Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("Test", message);
        }
    }
}
