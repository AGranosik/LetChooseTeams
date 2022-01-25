using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnit.DFM.Interfaces
{
    public interface IAppConfigurationSetUp
    {
        IAppConfigurationSetUp Environment(string env);
        IAppConfigurationSetUp ProjectName(string projectName);
        IWebHostEnvironment Build();
    }
}
