using LCT.Infrastructure.MessageBrokers.Interfaces;
using LCT.Infrastructure.MessageBrokers.Models;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Redis;

namespace LCT.Infrastructure.MessageBrokers
{
    internal class RedisMessageBroker : IMessageBroker
    {
        private readonly ConnectionMultiplexer _connection;
        public RedisMessageBroker(RedisSettings redisSettings)
        {
            _connection = ConnectionMultiplexer.Connect(redisSettings.ConnectionString, options =>
            {
                options.Password = redisSettings.Password;
            });
        }
        public async Task PublishAsync<T>(string groupId, T message)
        {
            var subscriber = _connection.GetSubscriber();

            //var clients = await subscriber.PublishAsync(destination, JsonConvert.SerializeObject(message), CommandFlags.FireAndForget);
            //Log.Information($@"Message sent via redis. Clients: {clients}. Channel: {destination}");
        }

        public async Task SubscribeAsync(MessageBrokerConnection connection)
        {
            //throw new NotImplementedException();
        }

        public async Task UnsubscribeAsync(MessageBrokerConnection connection)
        {
            //throw new NotImplementedException();
        }
    }
}
