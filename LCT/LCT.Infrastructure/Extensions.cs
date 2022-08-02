using LCT.Core.Entites.Tournaments.ValueObjects;
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
            => services.ConfigureMongo();

        private static IServiceCollection ConfigureMongo(this IServiceCollection services)
        {
            var mongoConfig = services.GetOptions<MongoSettings>("mongo");
            var mongoClient = new MongoClient(mongoConfig.ConnectionString);
            services.AddSingleton(mongoConfig);
            services.AddSingleton<IMongoClient>(mongoClient);
            services.AddSingleton<IPersistanceClient, MongoPersistanceClient>();
            services.AddSingleton(typeof(IRepository<>), typeof(AggregateRepository<>));
            AddIndexes(mongoClient, mongoConfig.DatabaseName);
            return services;
        }

        private static void AddIndexes(MongoClient client, string dbName)
        {
            var index = Builders<Name>.IndexKeys.Ascending(x => x.Value);
            client.GetDatabase(dbName).GetCollection<Name>("Tournament_TournamentName_index")
                .Indexes.CreateOne(index);
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
