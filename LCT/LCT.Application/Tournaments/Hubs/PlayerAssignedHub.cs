using Microsoft.AspNetCore.SignalR;

namespace LCT.Application.Tournaments.Hubs
{
    public class PlayerAssignedHub : Hub
    {
        public async Task SendMessage(string message)
        {
            // store clicked team somewhere
            // send to clients that some team were clicked
            await Clients.All.SendAsync("Test", message);
        }
    }
}
