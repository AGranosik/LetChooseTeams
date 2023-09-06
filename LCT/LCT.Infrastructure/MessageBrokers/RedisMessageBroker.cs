using System.Text.RegularExpressions;
using LCT.Infrastructure.ClientCommunication.Hubs;
using LCT.Infrastructure.MessageBrokers.Interfaces;
using LCT.Infrastructure.MessageBrokers.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly.Fallback;
using Polly;
using Serilog;
using StackExchange.Redis;
using Polly.Wrap;
using LCT.Core.RetryPolicies;

namespace LCT.Infrastructure.MessageBrokers
{
    internal class RedisMessageBroker : IMessageBroker
    {
        private ConnectionMultiplexer _connection;
        private readonly RedisSettings _settings;
        private static Dictionary<string, List<string>> _groupConnectionsDicitonary = new();
        private readonly IHubContext<TournamentHub> _hubContext;
        private static List<UnsentMessage> _unsentMessages = new();
        private static JsonSerializerSettings _serializeSettings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private static readonly AsyncFallbackPolicy _fallbackPolicy = Policy.Handle<Exception>()
            .FallbackAsync(async ct =>
            {
                // there gonna be alert to inform IT department about failure
                Log.Error("Error during trying to establish connection to Redis.");
                await Task.FromResult(true);
            });

        private readonly AsyncPolicyWrap _retryWrappedPolicy = Policy.WrapAsync(_fallbackPolicy, ConnectionPolicy.AsyncRetryForever);

        public RedisMessageBroker(RedisSettings redisSettings, IHubContext<TournamentHub> hubContext)
        {
            _settings = redisSettings;
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

            var connections = GetConnectionsIfGroupsExists(connection.GroupId);
            if(connections is null)
            {
                _groupConnectionsDicitonary.Add(connection.GroupId, new List<string> { connection.UserIdentifier });
                await _retryWrappedPolicy.ExecuteAsync(async () => {
                    var group = GetConnectionsIfGroupsExists(connection.GroupId);
                    if(group is not null)
                        await SubscribeAsync(connection.GroupId);
                });
            }
            else
            {
                connections.Add(connection.UserIdentifier);
            }
        }
        public async Task UnsubscribeAsync(MessageBrokerConnection connection)
        {
            if (!ConnectionValidation(connection))
                return;

            var connections = GetConnectionsIfGroupsExists(connection.GroupId);
            if (connections is null)
                return;

            connections.RemoveAll(c => c == connection.UserIdentifier);

            if(connections.Count == 0)
                await UnsubscribeAsync(connection.GroupId);
        }

        private async Task<long> TryPublishAsync<T>(string groupId, T message)
        {
            if (!_groupConnectionsDicitonary.Any(d => d.Key == groupId))
                Log.Warning($@"Misssing connection for {groupId}");

            var serializedMessage = SerilizeMessage(message);

            var queuedMessages = _unsentMessages.Where(um => true).OrderBy(um => um.CreationDate).ToList();
            _unsentMessages.RemoveAll(um => queuedMessages.Any(qm => qm.Id == um.Id));
            queuedMessages.Add(new UnsentMessage(groupId, serializedMessage));

            long result = 0;
            try
            {
                if(_connection is null || _connection.IsConnected)
                    result = await PublishMessagesAsync(queuedMessages);
                else
                    _unsentMessages.AddRange(queuedMessages);
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
            var subscriber = await GetSubscriber();
            long clients = 0;
            foreach (var queuedMessage in unsentMessages)
            {
                var singleCLients = await subscriber.PublishAsync(queuedMessage.GroupId, queuedMessage.SerializedMessage);
                if (singleCLients == 0)
                    await UnsubscribeAsync(queuedMessage.GroupId);

                clients += singleCLients;
            }
            return clients;
        }

        private static bool ConnectionValidation(MessageBrokerConnection connection)
            => connection.GroupId is not null && connection.UserIdentifier is not null;
        private async Task UnsubscribeAsync(string groupId)
        {
            var pubsub = await GetSubscriber();
            await pubsub.UnsubscribeAsync(groupId);
        }
        private async Task SubscribeAsync(string groupId)
        {
            var pubsub = await GetSubscriber();
            await pubsub.SubscribeAsync(groupId, (channel, message) =>
            {
                Log.Information($@"Message received. group id: {groupId}");
                _hubContext.Clients.All.SendCoreAsync(groupId, new[] { message.ToString() });
            });
        }

        private static List<string> GetConnectionsIfGroupsExists(string groupId)
        {
            var isValueExist = _groupConnectionsDicitonary.TryGetValue(groupId, out var result);

            return isValueExist ? result : null;
        }

        private static string SerilizeMessage<T>(T message)
            => JsonConvert.SerializeObject(message, _serializeSettings);

        private async Task TryOpenRedisConnection()
        {
            try
            {
                _connection = await ConnectionMultiplexer.ConnectAsync(_settings.ConnectionString, options =>
                {
                    options.Password = _settings.Password;
                    options.HeartbeatInterval = TimeSpan.FromSeconds(20);
                });
            }
            catch (Exception ex)
            {
                Log.Error($@"Redis connection failed.", ex);
                throw;
            }
        }

        private async Task<ISubscriber> GetSubscriber()
        {
            if (_connection is null || !_connection.IsConnected)
                await TryOpenRedisConnection();

            return _connection.GetSubscriber();
        }
    }

    internal class UnsentMessage {
        public  string GroupId { get; init; }
        public string SerializedMessage { get; init; }
        public Guid Id { get; init; }
        public DateTime CreationDate { get; init; }
        public UnsentMessage (string groupId, string serializedMessage)
        {
            GroupId = groupId;
            SerializedMessage = serializedMessage;
            Id = Guid.NewGuid();
            CreationDate = DateTime.Now;
        }
    
    }
}
