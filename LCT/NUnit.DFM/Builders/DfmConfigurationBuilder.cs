using LCT.Api;
using Microsoft.Extensions.Configuration;
using NUnit.DFM.Interfaces;

namespace NUnit.DFM.Builders
{
    internal class DfmConfigurationBuilder: IConfigurationBuilderSetup
    {
        public virtual IConfigurationRoot Create()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            string jsonFileName = string.IsNullOrEmpty(environment) ? "appsettings.json" : $"appsettings.{environment}.json";
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(jsonFileName, true, true)
                .AddEnvironmentVariables()
                .AddUserSecrets<Program>()
                .Build();
        }
    }
}
