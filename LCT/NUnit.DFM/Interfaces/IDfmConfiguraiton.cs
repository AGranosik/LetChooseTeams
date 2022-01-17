using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnit.DFM.Interfaces
{
    public interface IDfmConfiguraiton
    {
        IConfigurationBuilderSetup SetUpConfiguration();
        IDfmConfiguraiton SetConfiguration(IConfiguration configuration);
        IServiceCollectionSetUp SetUpServices();
        IDfmConfiguraiton SetServices(IServiceCollection services);
        IAppConfigurationSetUp AppConfiguration();
        IDfmConfiguraiton ApplyAppConfiguration(IWebHostEnvironment config);
    }
}
