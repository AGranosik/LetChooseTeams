using Microsoft.Extensions.Configuration;
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
        public async Task PublishAsync<T>(string destination, T message)
        {
            var subscriber = _connection.GetSubscriber();

            var clients = await subscriber.PublishAsync(destination, JsonConvert.SerializeObject(message), CommandFlags.FireAndForget);
            Log.Information($@"Message sent via redis. Clients: {clients}. Channel: {destination}");
        }

        public Task SubscribeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
