using LCT.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.DFM.Interfaces;

namespace NUnit.DFM
{
    public partial class Testing<TContext> : IAppConfigurationSetUp, IConfigurationBuilderSetup, IServiceCollectionSetUp
        where TContext : DbContext
    {

        public IConfigurationBuilderSetup SetBasePath(string basePath)
            => _configurationSetup.SetBasePath(basePath);
        public IConfigurationBuilderSetup SetEnvironment(string environment)
            => _configurationSetup.SetEnvironment(environment);
        public IConfigurationBuilderSetup AddEnvironmentVariables()
            => _configurationSetup.AddEnvironmentVariables();
        public IConfigurationRoot Create()
            => _configurationSetup.Create();
    }
}
