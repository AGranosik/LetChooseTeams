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
    public class Testing<TContext>
        where TContext: DbContext
    {
        private IConfigurationRoot _configuration;
        public IServiceScopeFactory _scopeFactory;
        /*
            configurationBuilderSection
            services section
            dbSection
            dbContextExtension
         
         */
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();

            var services = new ServiceCollection();

            var startup = new Startup(_configuration);

            services.AddSingleton(Mock.Of<IWebHostEnvironment>(w =>
                w.EnvironmentName == "Development" &&
                w.ApplicationName == "LCT.Api"));

            services.AddSingleton<IConfiguration>(_configuration);

            startup.ConfigureServices(services);

            _scopeFactory = services.BuildServiceProvider().GetService<IServiceScopeFactory>();

            EnsureDatabase();
        }

        private async Task EnsureDatabase()
        {
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetService<TContext>();

            context.Database.Migrate();
        }
    }
}