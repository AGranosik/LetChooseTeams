namespace LCT.Infrastructure.MessageBrokers
{
    internal interface IMessageBroker
    {
        Task PublishAsync<T>(string destination, T message);
        Task SubscribeAsync(); //tmp
    }
}
