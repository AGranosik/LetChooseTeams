using LCT.Application.Common.Interfaces;
using LCT.Infrastructure.MessageBrokers.Interfaces;
using Polly;

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
            var policy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(4),
                    TimeSpan.FromSeconds(8)
                });

            await policy.ExecuteAsync(() => _messageBroker.PublishAsync(destination, message));
        }
    }
}
