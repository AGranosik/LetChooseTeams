using Microsoft.AspNetCore.SignalR;

namespace LCT.Application.Tournaments.Hubs
{
    public class PlayerAssignedHub : Hub
    {
        public async Task SendMessage(Guid tournamentId, Guid playerId)
        {
            await Clients.Clients
        }
    }
}
