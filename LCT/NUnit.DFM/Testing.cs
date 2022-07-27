﻿using LCT.Api;
using LCT.Core.Entites;
using LCT.Infrastructure.Persistance.Mongo;
using LCT.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
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
        where TModel: Aggregate
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
            _scope = _scopeFactory.CreateScope();

        }

        [OneTimeTearDown]
        public virtual async Task OneTimeTearDown()
        {
            var mongoClient = _services.BuildServiceProvider().GetRequiredService<IMongoClient>();
            await mongoClient.DropDatabaseAsync("Lct_test");
        }

        [SetUp]
        public virtual void OneTimeSetuUp()
        {
            _scope = _scopeFactory.CreateScope();
        }

        [TearDown]
        public virtual async Task TearDownAsync()
        {
            var mongoClient = _scope.ServiceProvider.GetRequiredService<IMongoPersistanceClient>();
            await mongoClient.GetStream("TournamentStream").DeleteManyAsync(x => true);
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

        protected IRepository<TModel> GetRepository()
        {
            return _scope.ServiceProvider.GetRequiredService<IRepository<TModel>>();
        }

        protected IMongoClient GetMongoClient()
            => _scope.ServiceProvider.GetRequiredService<IMongoClient>();

        protected IRepository<TModel> GetRepository<TModel>()
            => _scope.ServiceProvider.GetRequiredService<IRepository<TModel>>();

        protected async Task AddAsync(TModel entity)
        {
            var dbContext = GetRepository();
            await dbContext.Save(entity);
        }

    }
}