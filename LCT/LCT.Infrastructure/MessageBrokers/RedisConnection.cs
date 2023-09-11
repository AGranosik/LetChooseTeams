using LCT.Infrastructure.MessageBrokers.Interfaces;
using Serilog;
using StackExchange.Redis;

namespace LCT.Infrastructure.MessageBrokers
{
    internal class RedisConnection : IRedisConnection
    {
        private RedisSettings _settings;
        private ConnectionMultiplexer _connection;
        public RedisConnection(RedisSettings settings)
        {
            _settings = settings;
        }
        public async Task<ISubscriber> GetSubscriber()
        {
            if (_connection is null || !_connection.IsConnected)
                await TryOpenRedisConnection();

            return _connection.GetSubscriber();
        }

        public bool IsOpened()
            => _connection is not null && _connection.IsConnected;

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
    }
}
