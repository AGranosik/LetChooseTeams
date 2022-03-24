using Microsoft.AspNetCore.SignalR;

namespace LCT.Application.Tournaments.Hubs
{
    public class PlayerAssignedHub : Hub
    {
        public async Task SendMessage(string message)
        {
            var clinets = Clients.All;
            await Clients.All.SendAsync("Test", message);
        }
    }
}
