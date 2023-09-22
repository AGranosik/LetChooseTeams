using StackExchange.Redis;

namespace LCT.Infrastructure.MessageBrokers.Interfaces
{
    public interface IRedisConnection
    {
        Task<ISubscriber> GetSubscriber();
        bool IsOpened();
    }
}
