using LCT.Infrastructure.ClientCommunication.Hubs;
using LCT.Infrastructure.MessageBrokers.Interfaces;
using LCT.Infrastructure.MessageBrokers.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly.Fallback;
using Polly;
using Serilog;
using Polly.Wrap;
using LCT.Core.RetryPolicies;

namespace LCT.Infrastructure.MessageBrokers
{
    public class RedisMessageBroker : IMessageBroker
    {
        private Dictionary<string, List<string>> _groupConnectionsDicitonary = new();
        private readonly IHubContext<TournamentHub> _hubContext;
        private List<UnsentMessage> _unsentMessages = new();
        private readonly IRedisConnection _redisConnection;
        private JsonSerializerSettings _serializeSettings = new()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        //cos tu sie odawala, ze 200 nie zwraca
        private static readonly AsyncFallbackPolicy _fallbackPolicy = Policy.Handle<Exception>()
            .FallbackAsync(async ct =>
            {
                // there gonna be alert to inform IT department about failure
                Log.Error("Error during trying to establish connection to Redis.");
                await Task.FromResult(true);
            });

        private readonly AsyncPolicyWrap _retryWrappedPolicy = Policy.WrapAsync(_fallbackPolicy, ConnectionPolicy.AsyncRetryForever);

        public RedisMessageBroker(IRedisConnection redisConnection, IHubContext<TournamentHub> hubContext)
        {
            _redisConnection = redisConnection;
            _hubContext = hubContext;
        }
        public async Task PublishAsync<T>(string groupId, T message)
        {
            var clients = await TryPublishAsync(groupId, message);
            Log.Information("Message sent via redis. Clients: {clients}. Channel: {groupId}", clients, groupId);
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

        // cos z operacjami na lsitach, mozliwe, ze czyszcza sobie watki nawzajem
        public async Task UnsubscribeAsync(MessageBrokerConnection connection)
        {
            if (!ConnectionValidation(connection))
                return;

            var connections = GetConnectionsIfGroupsExists(connection.GroupId);
            if (connections is null)
                return;

            connections.RemoveAll(c => c == connection.UserIdentifier);

            if(connections.Count == 0)
            {
                _groupConnectionsDicitonary.Remove(connection.GroupId);
                await UnsubscribeAsync(connection.GroupId);
            }
        }

        private async Task<long> TryPublishAsync<T>(string groupId, T message)
        {
            var serializedMessage = SerilizeMessage(message);

            var queuedMessages = GetUnsentMessages();
            queuedMessages.Add(new UnsentMessage(groupId, serializedMessage));

            long result = 0;
            try
            {
                if(_redisConnection.IsOpened())
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
            var subscriber = await _redisConnection.GetSubscriber();
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

        private List<UnsentMessage> GetUnsentMessages()
        {
            var queuedMessages = _unsentMessages.Where(um => true).OrderBy(um => um.CreationDate).ToList();
            _unsentMessages.RemoveAll(um => queuedMessages.Any(qm => qm.Id == um.Id));

            return queuedMessages;
        }

        private static bool ConnectionValidation(MessageBrokerConnection connection)
            => !string.IsNullOrEmpty(connection.GroupId) && !string.IsNullOrEmpty(connection.UserIdentifier);
        private async Task UnsubscribeAsync(string groupId)
        {
            var pubsub = await _redisConnection.GetSubscriber();
            await pubsub.UnsubscribeAsync(groupId);
        }
        private async Task SubscribeAsync(string groupId)
        {
            var pubsub = await _redisConnection.GetSubscriber();
            await pubsub.SubscribeAsync(groupId, (channel, message) =>
            {
                Log.Information("Message received. group id: {groupId}", groupId);
                _hubContext.Clients.All.SendCoreAsync(groupId, new[] { message.ToString() });
            });
        }

        private List<string> GetConnectionsIfGroupsExists(string groupId)
        {
            var isValueExist = _groupConnectionsDicitonary.TryGetValue(groupId, out var result);

            return isValueExist ? result : null;
        }

        private string SerilizeMessage<T>(T message)
            => JsonConvert.SerializeObject(message, _serializeSettings);
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
