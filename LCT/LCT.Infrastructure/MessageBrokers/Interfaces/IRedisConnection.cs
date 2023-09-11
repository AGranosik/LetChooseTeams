using StackExchange.Redis;

namespace LCT.Infrastructure.MessageBrokers.Interfaces
{
    internal interface IRedisConnection
    {
        Task<ISubscriber> GetSubscriber();
        bool IsOpened();
    }
}
