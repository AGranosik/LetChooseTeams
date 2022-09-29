using LCT.Domain.Common.BaseTypes;
using Microsoft.Extensions.Configuration;
using NUnit.DFM.Interfaces;

namespace NUnit.DFM
{
    public partial class Testing<TModel> : IAppConfigurationSetUp, IConfigurationBuilderSetup, IServiceCollectionSetUp
        where TModel : IAgregateRoot
    {
        public IConfigurationRoot Create()
            => _configurationSetup.Create();
    }
}
