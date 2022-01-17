﻿using Microsoft.EntityFrameworkCore;
using NUnit.DFM.Builders;
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

        public IServiceCollectionSetUp SwapSingleton<TSingleton>(TSingleton singleton)
            where TSingleton : class
            => _builder.SwapSingleton(singleton);

        public IServiceCollectionSetUp AddSingleton<TSingleton>(TSingleton singleton)
            where TSingleton : class
            => _builder.AddSingleton(singleton);

        public IServiceCollectionSetUp Remove<TService>()
            where TService : class
        => _builder.Remove<TService>();

        public IServiceCollectionSetUp AddScoped<TScoped>(TScoped scoped)
            where TScoped : class
            => _builder.AddScoped(scoped);

        public IServiceCollectionSetUp SwapScoped<TScoped>(TScoped scoped)
            where TScoped : class
            => _builder.SwapScoped(scoped);

        public IServiceCollectionSetUp AddTransient<TTransient>(TTransient transient)
            where TTransient : class
            => _builder.AddTransient(transient);

        public IServiceCollectionSetUp SwapTransient<TTransient>(TTransient transient)
            where TTransient : class
            => _builder.SwapTransient(transient);
    }
}