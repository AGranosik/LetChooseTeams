using LCT.Infrastructure.ClientCommunication.Hubs;
using LCT.Infrastructure.MessageBrokers.Interfaces;
using LCT.Infrastructure.MessageBrokers.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using StackExchange.Redis;


namespace LCT.Infrastructure.MessageBrokers
{
    internal class RedisMessageBroker : IMessageBroker
    {
        private readonly ConnectionMultiplexer _connection;
        private static Dictionary<string, List<string>> _groupConnectionsDicitonary = new();
        private readonly IHubContext<TournamentHub> _hubContext;
        public RedisMessageBroker(RedisSettings redisSettings, IHubContext<TournamentHub> hubContext)
        {
            _connection = ConnectionMultiplexer.Connect(redisSettings.ConnectionString, options =>
            {
                options.Password = redisSettings.Password;
            });
            _hubContext = hubContext;
        }
        public async Task PublishAsync<T>(string groupId, T message)
        {
            var subscriber = _connection.GetSubscriber();

            var clients = await subscriber.PublishAsync(groupId, JsonConvert.SerializeObject(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }), CommandFlags.FireAndForget);
            Log.Information($@"Message sent via redis. Clients: {clients}. Channel: {groupId}");
        }

        public async Task SubscribeAsync(MessageBrokerConnection connection)
        {
            if (!ConnectionValidation(connection))
                return;

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
            if (ConnectionValidation(connection))
                return;

            var connections = GetCeonnectionsIfGroupsExists(connection.GroupId);
            if (connections is null)
                return;

            connections.Remove(connection.UserIdentifier);

            if(connections.Count == 0)
                await UnsubscribeAsync(connection.GroupId);
        }

        private static bool ConnectionValidation(MessageBrokerConnection connection)
            => connection.GroupId is not null && connection.UserIdentifier is not null;
        private async Task UnsubscribeAsync(string groupId)
        {
            var pubsub = _connection.GetSubscriber();
            await pubsub.UnsubscribeAsync(groupId);
        }

        private async Task SubscribeAsync(string groupId)
        {
            var pubsub = _connection.GetSubscriber();
            await pubsub.SubscribeAsync(groupId, (channel, message) =>
            {
                Log.Information($@"Message received. group id: {groupId}");
                _hubContext.Clients.All.SendCoreAsync(groupId, new[] { message.ToString() });
            });
        }

        private static List<string> GetCeonnectionsIfGroupsExists(string groupId)
        {
            var result = new List<string>();
            var isValueExist = _groupConnectionsDicitonary.TryGetValue(groupId, out result);

            return isValueExist ? result : null;
        }

    }
}
