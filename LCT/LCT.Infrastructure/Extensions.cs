using LCT.Application.Common.Configs;
using LCT.Application.Common.Interfaces;
using LCT.Domain.Aggregates.TournamentAggregate.Events;
using LCT.Domain.Common.BaseTypes;
using LCT.Domain.Common.Interfaces;
using LCT.Infrastructure.ClientCommunication;
using LCT.Infrastructure.MessageBrokers;
using LCT.Infrastructure.MessageBrokers.Interfaces;
using LCT.Infrastructure.Persistance.ActionsStorage;
using LCT.Infrastructure.Persistance.EventsStorage;
using LCT.Infrastructure.Persistance.EventsStorage.UniqnessFactories;
using LCT.Infrastructure.Repositories;
using LCT.Infrastructure.Repositories.Actions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace LCT.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
            => services.ConfigureMongo()
                .ConfigureRedis()
                .ConfigureWebsockets();

        private static IServiceCollection ConfigureMongo(this IServiceCollection services)
        {
            //storage of events and indexes
            var mongoConfig = services.GetOptions<MongoSettings>("mongo");
            var mongoClient = new MongoClient(mongoConfig.ConnectionString);
            services.AddSingleton(mongoConfig);
            services.AddSingleton<IMongoClient>(mongoClient);
            services.AddSingleton<IPersistanceClient, MongoPersistanceClient>();
            services.AddSingleton(typeof(IAggregateRepository<>), typeof(AggregateRepository<>));
            services.AddSingleton(typeof(ILctActionRepository<,>), typeof(LctActionRepository<,>));
            services.AddSingleton<IActionStorageClient, MongoActionStorageClient>();
            services.AddSingleton<IUniqnessIndexExecutor, UniqnessIndexExecutor>();
            RegisterDomainEvents();
            services.ConfigureFrontendUrl();

            return services;
        }

        private static IServiceCollection ConfigureRedis(this IServiceCollection services)
        {
            var redisConfig = services.GetOptions<RedisSettings>("Redis");
            services.AddSingleton(redisConfig);

            services.AddSingleton<IMessageBroker, RedisMessageBroker>();
            return services;
        }

        private static IServiceCollection ConfigureWebsockets(this IServiceCollection services)
        {
            services.AddSingleton<IClientCommunicationService, WebSocketClientCommunicationService>();
            return services;
        }

        private static void RegisterDomainEvents()
        {
            RegisterDomainEvent<TournamentCreatedDomainEvent>();
            RegisterDomainEvent<PlayerAddedDomainEvent>();
            RegisterDomainEvent<TeamSelectedDomainEvent>();
            RegisterDomainEvent<DrawTeamEvent>();
        }

        private static void RegisterDomainEvent<T>()
            where T : DomainEvent
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
                BsonClassMap.RegisterClassMap<T>();
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

        private static IServiceCollection ConfigureFrontendUrl(this IServiceCollection services)
        {
            // generate based on who send request?
            var fe = services.GetOptions<FrontendConfiguration>("fe");
            services.AddSingleton(fe);

            return services;
        }
    }
}
