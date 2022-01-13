using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnit.DFM.Interfaces
{
    public interface IConfigurationBuilderSetup
    {
        IConfigurationBuilderSetup SetBasePath(string basePath);
        IConfigurationBuilderSetup AddJsonFile(string path, bool optional, bool realoadOnChange);
        IConfigurationBuilderSetup AddEnvironmentVariables();
        IConfigurationRoot Build();
    }
}
