using LCT.Core.Entites;
using Microsoft.Extensions.Configuration;
using NUnit.DFM.Interfaces;

namespace NUnit.DFM
{
    public partial class Testing<TModel> : IAppConfigurationSetUp, IConfigurationBuilderSetup, IServiceCollectionSetUp
        where TModel : Aggregate
    {
        public IConfigurationRoot Create()
            => _configurationSetup.Create();
    }
}
