﻿using LCT.Domain.Common.BaseTypes;
using Microsoft.AspNetCore.Hosting;
using NUnit.DFM.Interfaces;

namespace NUnit.DFM
{
    public partial class Testing<TModel> : IAppConfigurationSetUp, IConfigurationBuilderSetup, IServiceCollectionSetUp
        where TModel : IAgregateRoot
    {
        public IAppConfigurationSetUp Environment(string env)
            => _appConfiguration.Environment(env);
        public IAppConfigurationSetUp ProjectName(string projectName)
            => _appConfiguration.ProjectName(projectName);
        public IWebHostEnvironment Build()
            => _appConfiguration.Build();
    }
}
