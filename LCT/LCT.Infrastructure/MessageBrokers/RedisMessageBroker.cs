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
        private static Dictionary<string, List<string>> _groupConnectionsDicitonary = new();
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

            var clients = await subscriber.PublishAsync(groupId, JsonConvert.SerializeObject(message), CommandFlags.FireAndForget);
            Log.Information($@"Message sent via redis. Clients: {clients}. Channel: {groupId}");
        }

        public async Task SubscribeAsync(MessageBrokerConnection connection)
        {
            var connections = GetCeonnectionsIfGroupsExists(connection.GroupId);
            if(connections is null)
            {
                _groupConnectionsDicitonary.Add(connection.GroupId, new List<string> { connection.UserIdentifier });
                await SubscribeAsync(connection.GroupId);
            }
            else
            {
                if(!connections.Any(c => c == connection.UserIdentifier))
                    connections.Add(connection.UserIdentifier);
            }
        }

        public async Task UnsubscribeAsync(MessageBrokerConnection connection)
        {
            //throw new NotImplementedException();
        }

        private async Task SubscribeAsync(string groupId)
        {
            var pubsub = _connection.GetSubscriber();
            await pubsub.SubscribeAsync(groupId, (channel, message) => Console.WriteLine("test test test " + message));
        }

        private static List<string> GetCeonnectionsIfGroupsExists(string groupId)
        {
            var result = new List<string>();
            var isValueExist = _groupConnectionsDicitonary.TryGetValue(groupId, out result);

            return isValueExist ? result : null;
        }

    }
}
