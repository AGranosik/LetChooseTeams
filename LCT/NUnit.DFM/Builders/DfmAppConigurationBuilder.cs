using Microsoft.AspNetCore.Hosting;
using Moq;
using NUnit.DFM.Interfaces;

namespace NUnit.DFM.Builders
{
    public class DfmAppConigurationBuilder : IAppConfigurationSetUp
    {
        private string _env;
        private string _projectName;

        public DfmAppConigurationBuilder()
        {
            _env = "Development";
        }

        public IWebHostEnvironment Build()
        {
            return Mock.Of<IWebHostEnvironment>(w =>
                w.EnvironmentName == _env &&
                w.ApplicationName == _projectName);
        }

        public IAppConfigurationSetUp Environment(string env)
        {
            _env = env;
            return this;
        }

        public IAppConfigurationSetUp ProjectName(string projectName)
        {
            _projectName = projectName;
            return this;
        }
    }
}
