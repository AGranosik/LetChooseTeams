using LCT.Infrastructure.MessageBrokers.Models;

namespace LCT.Infrastructure.MessageBrokers.Interfaces
{
    public interface IMessageBroker
    {
        Task PublishAsync<T>(string groupId, T message);
        Task SubscribeAsync(MessageBrokerConnection connection);
        Task UnsubscribeAsync(MessageBrokerConnection connection);
    }
}
