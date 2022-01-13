using LCT.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace NUnit.DFM
{
    [SetUpFixture]
    public class Testing<TContext, TProgram>
        where TContext: DbContext
        where TProgram: class, new()
    {
        private static IConfigurationRoot _configuration;
        private static IServiceScopeFactory _scopeFactory;

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json", true, true)
            //    .AddEnvironmentVariables();

            _configuration = builder.Build();

            var services = new ServiceCollection();

            var startup = new Startup(_configuration);

            services.AddSingleton(Mock.Of<IWebHostEnvironment>(w =>
                w.EnvironmentName == "Development" &&
                w.ApplicationName == "CleanTesting.WebUI"));

            startup.ConfigureServices(services);

            _scopeFactory = services.BuildServiceProvider().GetService<IServiceScopeFactory>();

            EnsureDatabase();
        }

        private async Task EnsureDatabase()
        {
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetService<TContext>();

            context.Database
        }

        public static async Task AddAsync<TEntity>(TEntity entity)
            where TEntity : class
        {
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetService<TContext>();

            context.Add(entity);

            await context.SaveChangesAsync();
        }
    }
}