using Microsoft.Extensions.Configuration;

namespace NUnit.DFM.Interfaces
{
    public interface IConfigurationBuilderSetup
    { 
        IConfigurationBuilderSetup SetBasePath(string basePath);
        IConfigurationBuilderSetup AddEnvironmentVariables();
        IConfigurationRoot Create();
    }
}
