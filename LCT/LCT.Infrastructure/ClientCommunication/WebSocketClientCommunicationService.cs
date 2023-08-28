using LCT.Application.Common.Interfaces;
using LCT.Infrastructure.MessageBrokers.Interfaces;

namespace LCT.Infrastructure.ClientCommunication
{
    internal class WebSocketClientCommunicationService : IClientCommunicationService
    {
        private readonly IMessageBroker _messageBroker;

        public WebSocketClientCommunicationService(IMessageBroker messsageBroker)
        {
            _messageBroker = messsageBroker;
        }

        public async Task SendAsync<T>(string destination, T message, CancellationToken cancellationToken)
            where T : class
        {
            await _messageBroker.PublishAsync(destination, message);
        }
    }
}
