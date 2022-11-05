using LCT.Api;
using LCT.Application.Common.Interfaces;
using LCT.Domain.Aggregates.TournamentAggregate.Services;
using LCT.Domain.Common.BaseTypes;
using LCT.Domain.Common.Interfaces;
using LCT.Infrastructure.Persistance.Mongo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NUnit.DFM.Builders;
using NUnit.DFM.Interfaces;
using NUnit.Framework;

namespace NUnit.DFM
{
    [SetUpFixture]
    public partial class Testing<TModel>: IAppConfigurationSetUp, IConfigurationBuilderSetup, IServiceCollectionSetUp
        where TModel: IAgregateRoot
    {
        private IServiceScopeFactory _scopeFactory;
        protected IServiceScope _scope;
        private readonly IServiceCollection _services = new ServiceCollection();
        private readonly IServiceCollectionSetUp _builder;
        private readonly IAppConfigurationSetUp _appConfiguration;
        private readonly IConfigurationBuilderSetup _configurationSetup;
        private readonly List<string> _tableToTruncate = new List<string>();
        private string _dbName;

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
            _scope = _scopeFactory.CreateScope();
            var mongoSettings = _scope.ServiceProvider.GetRequiredService<MongoSettings>();
            _dbName = mongoSettings.DatabaseName;

        }

        [OneTimeTearDown]
        public virtual async Task OneTimeTearDown()
        {
            var mongoClient = _services.BuildServiceProvider().GetRequiredService<IMongoClient>();
            await mongoClient.DropDatabaseAsync(_dbName);
        }

        [SetUp]
        public virtual void OneTimeSetuUp()
        {
            _scope = _scopeFactory.CreateScope();
        }

        [TearDown]
        public virtual async Task TearDownAsync()
        {
            var mongoClient = _scope.ServiceProvider.GetRequiredService<IMongoClient>();
            foreach (var collection in _tableToTruncate)
                mongoClient.GetDatabase(_dbName).DropCollection(collection);

            _scope.Dispose();
        }

        protected Testing<TModel> AddTableToTruncate(string name)
        {
            _tableToTruncate.Add(name);
            return this;
        }

        protected Testing<TModel> AddTablesToTruncate(List<string> tables)
        {
            _tableToTruncate.AddRange(tables);
            return this;
        }

        protected IAggregateRepository<TModel> GetRepository()
        {
            return _scope.ServiceProvider.GetRequiredService<IAggregateRepository<TModel>>();
        }

        protected IPersistanceClient GetPersistanceClient()
        {
            return _scope.ServiceProvider.GetRequiredService<IPersistanceClient>();
        }

        protected ITournamentDomainService GetTournamentDomainService()
        {
            return _scope.ServiceProvider.GetRequiredService<ITournamentDomainService>();
        }

        protected IMongoClient GetMongoClient()
            => _scope.ServiceProvider.GetRequiredService<IMongoClient>();

        protected IAggregateRepository<TModel> GetRepository<TModel>()
            where TModel : IAgregateRoot
            => _scope.ServiceProvider.GetRequiredService<IAggregateRepository<TModel>>();

        protected async Task SaveAsync(TModel entity, int version = 0)
        {
            var dbContext = GetRepository();
            await dbContext.SaveAsync(entity, version);
        }

    }
}