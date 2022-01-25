using LCT.Api;
using LCT.Infrastructure.EF;
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
    public partial class Testing<TContext>: IAppConfigurationSetUp, IConfigurationBuilderSetup, IServiceCollectionSetUp, ITestsConfiguration
        where TContext: DbContext
    {
        protected IServiceScopeFactory _scopeFactory;
        private readonly IServiceCollection _services = new ServiceCollection();
        private readonly IServiceCollectionSetUp _builder;
        private readonly IAppConfigurationSetUp _appConfiguration;
        private readonly IConfigurationBuilderSetup _configurationSetup;

        public Testing()
        {
            _builder = new DfmServiceCollectionBuilder();
            _appConfiguration = new DfmAppConigurationBuilder();
            _configurationSetup = new DfmConfigurationBuilder();
        }
        [OneTimeSetUp]
        public async Task RunBeforeAnyTests()
        {
            var configuration = _configurationSetup.Create();
            var startup = new Startup(configuration);

            _services.AddSingleton<IConfiguration>(configuration);
            _services.AddSingleton(_appConfiguration.Build());

            startup.ConfigureServices(_services);
            _builder.Create(_services);

            _scopeFactory = _services.BuildServiceProvider().GetService<IServiceScopeFactory>();

            await EnsureDatabase();
        }

        private async Task EnsureDatabase()
        {
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetService<TContext>();

            await context.Database.MigrateAsync();
        }

        [OneTimeTearDown]
        public async Task GlobalTeardown()
        {
            using var scope = _scopeFactory.CreateScope();

            var context = scope.ServiceProvider.GetService<TContext>();
            await context.Database.EnsureDeletedAsync();
        }

    }
}