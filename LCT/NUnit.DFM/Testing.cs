using LCT.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.DFM.Builders;
using NUnit.DFM.Interfaces;
using NUnit.Framework;

namespace NUnit.DFM
{
    [SetUpFixture]
    public class Testing<TContext>: IDfmConfiguraiton
        where TContext: DbContext
    {
        private IConfiguration _configuration;
        protected IServiceScopeFactory _scopeFactory;
        private IConfigurationBuilderSetup _environmentConfigurationBuilder;

        public Testing()
        {
            
        }
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            _configuration ??= _environmentConfigurationBuilder.Build();

            var services = new ServiceCollection();

            var startup = new Startup(_configuration);

            services.AddSingleton(Mock.Of<IWebHostEnvironment>(w =>
                w.EnvironmentName == "Development" &&
                w.ApplicationName == "LCT.Api"));

            services.AddSingleton(_configuration);

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

        public IConfigurationBuilderSetup Configure()
        {
            _environmentConfigurationBuilder ??= new DfmConfigurationBuilder();
            return _environmentConfigurationBuilder;
        }

        public IConfigurationBuilderSetup SetConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
            return _environmentConfigurationBuilder;
        }
    }
}