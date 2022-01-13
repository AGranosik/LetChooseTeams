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
        private IServiceCollection _services;

        public Testing()
        {
            
        }
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {

            var startup = new Startup(_configuration);

            _services.AddSingleton(Mock.Of<IWebHostEnvironment>(w =>
                w.EnvironmentName == "Development" &&
                w.ApplicationName == "LCT.Api"));

            _services.AddSingleton(_configuration);

            startup.ConfigureServices(_services);
            //should modify this list of services isntead of creating new one

            _scopeFactory = _services.BuildServiceProvider().GetService<IServiceScopeFactory>();

            EnsureDatabase();
        }

        private async Task EnsureDatabase()
        {
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetService<TContext>();

            context.Database.Migrate();
        }

        public IConfigurationBuilderSetup SetUpConfiguration()
        {
            return new DfmConfigurationBuilder();
        }

        public IDfmConfiguraiton SetConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
            return this;
        }
        public IServiceCollectionSetUp SetUpServices()
        {
            return new DfmServiceCollectionBuilder();
        }

        public IDfmConfiguraiton SetServices(IServiceCollection services)
        {
            _services = services;
            return this;
        }
    }
}