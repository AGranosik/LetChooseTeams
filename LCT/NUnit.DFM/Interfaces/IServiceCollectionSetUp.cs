using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnit.DFM.Interfaces
{
    public interface IServiceCollectionSetUp
    {
        IServiceCollectionSetUp SwapSingleton<TSingleton>(TSingleton singleton)
            where TSingleton : class;
        IServiceCollectionSetUp AddSingleton<TSingleton>(TSingleton singleton)
            where TSingleton : class;
        IServiceCollectionSetUp Remove<TService>()
            where TService : class;

        IServiceCollectionSetUp AddScoped<TScoped>(TScoped scoped)
            where TScoped: class;
        IServiceCollectionSetUp SwapScoped<TScoped>(TScoped scoped)
            where TScoped : class;

        IServiceCollectionSetUp AddTransient<TTransient>(TTransient transient)
            where TTransient : class;
        IServiceCollectionSetUp SwapTransient<TTransient>(TTransient transient)
            where TTransient : class;

        IServiceCollection Create(IServiceCollection services);
    }
}
