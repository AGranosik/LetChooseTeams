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
    public partial class Testing<TContext>: IAppConfigurationSetUp, IConfigurationBuilderSetup, IServiceCollectionSetUp
        where TContext: DbContext
    {
        protected IServiceScopeFactory _scopeFactory;
        protected IServiceScope _scope;
        private readonly IServiceCollection _services = new ServiceCollection();
        private readonly IServiceCollectionSetUp _builder;
        private readonly IAppConfigurationSetUp _appConfiguration;
        private readonly IConfigurationBuilderSetup _configurationSetup;
        private readonly List<string> _tableToTruncate = new List<string>();

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

        [SetUp]
        public virtual void OneTimeSetuUp()
        {
            _scope = _scopeFactory.CreateScope();
        }

        [TearDown]
        public virtual async Task TearDownAsync()
        {
            var dbContext = GetDbContext();
            foreach (var table in _tableToTruncate)
                await dbContext.Database.ExecuteSqlRawAsync($@"DELETE FROM {table}");
            _scope.Dispose();
        }

        protected Testing<TContext> AddTableToTruncate(string name)
        {
            _tableToTruncate.Add(name);
            return this;
        }

        protected Testing<TContext> AddTablesToTruncate(List<string> tables)
        {
            _tableToTruncate.AddRange(tables);
            return this;
        }

        protected TContext GetDbContext()
        {
            return _scope.ServiceProvider.GetService<TContext>();
        }

        protected async Task AddAsync<TEntity>(TEntity entity)
        {
            var dbContext = GetDbContext();
            await dbContext.AddAsync(entity);
            await dbContext.SaveChangesAsync();
        }

    }
}