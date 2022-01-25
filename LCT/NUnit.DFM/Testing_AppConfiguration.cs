using LCT.Infrastructure.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using NUnit.DFM.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnit.DFM
{
    public partial class Testing<TContext> : IAppConfigurationSetUp, IConfigurationBuilderSetup, IServiceCollectionSetUp
        where TContext : DbContext
    {
        public IAppConfigurationSetUp Environment(string env)
            => _appConfiguration.Environment(env);
        public IAppConfigurationSetUp ProjectName(string projectName)
            => _appConfiguration.ProjectName(projectName);
        public IWebHostEnvironment Build()
            => _appConfiguration.Build();
    }
}
