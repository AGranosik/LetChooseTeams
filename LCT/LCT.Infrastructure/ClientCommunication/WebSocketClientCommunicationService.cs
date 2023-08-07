using LCT.Application.Common.Interfaces;
using LCT.Infrastructure.ClientCommunication.Hubs;
using LCT.Infrastructure.MessageBrokers.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace LCT.Infrastructure.ClientCommunication
{
    internal class WebSocketClientCommunicationService : IClientCommunicationService
    {
        private readonly IHubContext<TournamentHub> _hubContext;
        private readonly IMessageBroker _messageBroker;

        public WebSocketClientCommunicationService(IHubContext<TournamentHub> hubContext, IMessageBroker messsageBroker)
        {
            _hubContext = hubContext;
            _messageBroker = messsageBroker;
        }

        public async Task SendAsync<T>(string destination, T message, CancellationToken cancellationToken)
            where T : class
        {
            await _messageBroker.PublishAsync(destination, message);
        }
    }
}
