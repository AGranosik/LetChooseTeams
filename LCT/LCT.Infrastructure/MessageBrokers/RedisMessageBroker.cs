using System.Text.RegularExpressions;
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
        private ConnectionMultiplexer _connection;
        private readonly RedisSettings _settings;
        private static Dictionary<string, List<string>> _groupConnectionsDicitonary = new();
        private readonly IHubContext<TournamentHub> _hubContext;
        private List<UnsentMessage> _unsentMessages = new List<UnsentMessage>();

        public RedisMessageBroker(RedisSettings redisSettings, IHubContext<TournamentHub> hubContext)
        {
            _settings = redisSettings;
            TryOpenRedisConnection();
            _hubContext = hubContext;
        }
        public async Task PublishAsync<T>(string groupId, T message)
        {
            var clients = await TryPublishAsync(groupId, message);
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

        private async Task<long> TryPublishAsync<T>(string groupId, T message)
        {
            if (!_groupConnectionsDicitonary.Any(d => d.Key == groupId))
                Log.Warning($@"Misssing connection for {groupId}");

            var serializedMessage = JsonConvert.SerializeObject(message, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            var queuedMessages = _unsentMessages.Where(um => um.GroupId == groupId).ToList();
            _unsentMessages.RemoveAll(um => queuedMessages.Any(qm => qm.Id == um.Id));
            queuedMessages.Add(new UnsentMessage(groupId, serializedMessage));
            long result = 0;
            try
            {
                result = await PublishMessagesAsync(queuedMessages);
            }
            catch (Exception ex)
            {
                _unsentMessages.AddRange(queuedMessages);
                Log.Error(ex.ToString());
            }

            return result;
        }

        private async Task<long> PublishMessagesAsync(List<UnsentMessage> unsentMessages)
        {
            var subscriber = GetSubscriber();
            long clients = 0;
            foreach (var queuedMessage in unsentMessages)
            {
                var singleCLients = await subscriber.PublishAsync(queuedMessage.GroupId, queuedMessage.SerializedMessage, CommandFlags.FireAndForget);
                clients += singleCLients;
            }
            return clients;
        }

        private static bool ConnectionValidation(MessageBrokerConnection connection)
            => connection.GroupId is not null && connection.UserIdentifier is not null;
        private async Task UnsubscribeAsync(string groupId)
        {
            var pubsub = GetSubscriber();
            await pubsub.UnsubscribeAsync(groupId);
        }

        private async Task SubscribeAsync(string groupId)
        {
            var pubsub = GetSubscriber();
            await pubsub.SubscribeAsync(groupId, (channel, message) =>
            {
                Log.Information($@"Message received. group id: {groupId}");
                _hubContext.Clients.All.SendCoreAsync(groupId, new[] { message.ToString() });
            });
        }

        private static List<string> GetCeonnectionsIfGroupsExists(string groupId)
        {
            var isValueExist = _groupConnectionsDicitonary.TryGetValue(groupId, out var result);

            return isValueExist ? result : null;
        }

        private void TryOpenRedisConnection()
        {
            try
            {
                _connection = ConnectionMultiplexer.Connect(_settings.ConnectionString, options =>
                {
                    options.Password = _settings.Password;
                    options.HeartbeatInterval = TimeSpan.FromSeconds(20);
                });
            }
            catch (Exception ex)
            {
                Log.Error($@"Redis connection failed.", ex);
            }
        }

        private ISubscriber GetSubscriber()
        {
            if (_connection is null || !_connection.IsConnected)
                TryOpenRedisConnection();

            return _connection.GetSubscriber();
        }
    }

    internal class UnsentMessage {
        public  string GroupId { get; init; }
        public string SerializedMessage { get; init; }
        public Guid Id { get; init; }
        public UnsentMessage (string groupId, string serializedMessage)
        {
            GroupId = groupId;
            SerializedMessage = serializedMessage;
            Id = Guid.NewGuid();
        }
    
    } 
}
