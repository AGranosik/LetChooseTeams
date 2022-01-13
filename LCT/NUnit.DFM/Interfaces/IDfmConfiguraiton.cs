using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnit.DFM.Interfaces
{
    public interface IDfmConfiguraiton
    {
        IConfigurationBuilderSetup Configure();
        IConfigurationBuilderSetup SetConfiguration(IConfiguration configuration);
    }
}
