using Microsoft.Extensions.Configuration;
using NUnit.DFM.Interfaces;

namespace NUnit.DFM.Builders
{
    internal class DfmConfigurationBuilder: IConfigurationBuilderSetup
    {
        private string _basePath;
        private string _environment;
        private bool _environmentVariables;

        public IConfigurationBuilderSetup SetBasePath(string basePath)
        {
            _basePath = basePath;
            return this;
        }

        public virtual IConfigurationBuilderSetup SetEnvironment(string environment)
        {
            _environment = environment;
            return this;
        }

        public virtual IConfigurationBuilderSetup AddEnvironmentVariables()
        {
            _environmentVariables = true;
            return this;
        }

        public virtual IConfigurationRoot Create()
        {
            string jsonFileName = string.IsNullOrEmpty(_environment) ? "appsettings.json" : $"appsettings.{_environment}.json";
            return new ConfigurationBuilder()
                .SetBasePath(_basePath ?? Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
