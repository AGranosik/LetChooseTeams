using EventStore.ClientAPI;
using LCT.Infrastructure.EF;
using LCT.Infrastructure.EventSourcing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LCT.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            var options = services.GetOptions<EfOptions>("sql");
            services.AddDbContext<LctDbContext>(x => x.UseSqlServer(options.ConnectionString));

            var esOptions = services.GetOptions<EsOptions>("EventStore");
            var eventStoreConnection = EventStoreConnection.Create(
                    connectionString: esOptions.ConnectionString,
                    builder: ConnectionSettings.Create().KeepReconnecting(),
                    connectionName: esOptions.ConnectionName);

            eventStoreConnection.ConnectAsync().GetAwaiter().GetResult();

            services.AddSingleton(eventStoreConnection);

            return services;
        }

        public static T GetOptions<T>(this IServiceCollection services, string sectionName) where T : new()
        {
            using var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            return configuration.GetOptions<T>(sectionName);
        }

        public static T GetOptions<T>(this IConfiguration configuration, string sectionName) where T : new()
        {
            var options = new T();
            configuration.GetSection(sectionName).Bind(options);
            return options;
        }
    }
}
