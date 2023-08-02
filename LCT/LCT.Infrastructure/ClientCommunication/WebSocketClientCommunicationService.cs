using LCT.Application.Common.Interfaces;
using LCT.Infrastructure.ClientCommunication.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace LCT.Infrastructure.ClientCommunication
{
    public class WebSocketClientCommunicationService : IClientCommunicationService
    {
        private readonly IHubContext<TournamentHub> _hubContext;

        public WebSocketClientCommunicationService(IHubContext<TournamentHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendAsync<T>(string destination, T message, CancellationToken cancellationToken)
            where T : class
        {
            await _hubContext.Clients.All.SendCoreAsync(destination, new[] { message } , cancellationToken);
        }
    }
}
