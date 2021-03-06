using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NUnit.DFM.Interfaces;

namespace NUnit.DFM
{
    public partial class Testing<TContext> : IAppConfigurationSetUp, IConfigurationBuilderSetup, IServiceCollectionSetUp
        where TContext : DbContext
    {
        public IConfigurationRoot Create()
            => _configurationSetup.Create();
    }
}
