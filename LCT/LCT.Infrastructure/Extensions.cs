﻿using EventStore.ClientAPI;
using LCT.Infrastructure.EF;
using LCT.Infrastructure.EventSourcing;
using LCT.Infrastructure.Persistance.Mongo;
using LCT.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace LCT.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            var options = services.GetOptions<EfOptions>("sql");
            services.AddDbContext<LctDbContext>(x => x.UseSqlServer(options.ConnectionString), ServiceLifetime.Singleton)
                .ConfigureMongo();

            return services;
        }

        private static IServiceCollection ConfigureMongo(this IServiceCollection services)
        {
            var mongoConfig = services.GetOptions<MongoSettings>("mongo");
            var mongoClient = new MongoClient(mongoConfig.ConnectionString);
            services.AddSingleton(mongoConfig);
            services.AddSingleton<IMongoClient>(mongoClient);
            services.AddSingleton<IMongoPersistanceClient, MongoPersistanceClient>();
            services.AddSingleton(typeof(IRepository<>), typeof(AggregateRepository<>));
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
