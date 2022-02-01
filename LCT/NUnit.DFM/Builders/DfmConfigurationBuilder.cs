using Microsoft.Extensions.Configuration;
using NUnit.DFM.Interfaces;

namespace NUnit.DFM.Builders
{
    internal class DfmConfigurationBuilder: IConfigurationBuilderSetup
    {
        private string _basePath;
        private bool _environmentVariables;

        public IConfigurationBuilderSetup SetBasePath(string basePath)
        {
            _basePath = basePath;
            return this;
        }

        public virtual IConfigurationBuilderSetup AddEnvironmentVariables()
        {
            _environmentVariables = true;
            return this;
        }

        public virtual IConfigurationRoot Create()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            string jsonFileName = string.IsNullOrEmpty(environment) ? "appsettings.json" : $"appsettings.{environment}.json";
            return new ConfigurationBuilder()
                .SetBasePath(_basePath ?? Directory.GetCurrentDirectory())
                .AddJsonFile(jsonFileName, true, true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
