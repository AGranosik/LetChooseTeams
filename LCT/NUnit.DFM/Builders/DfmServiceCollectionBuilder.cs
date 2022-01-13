using Microsoft.Extensions.DependencyInjection;
using NUnit.DFM.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnit.DFM.Builders
{
    public class DfmServiceCollectionBuilder: IServiceCollectionSetUp
    {
        private IServiceCollection _services;
        public DfmServiceCollectionBuilder()
        {
            _services = new ServiceCollection();
        }

        public IServiceCollectionSetUp AddScoped<TScoped>(TScoped scoped)
            where TScoped : class
        {
            _services.AddScoped(typeof(TScoped));
            return this;
        }

        public IServiceCollectionSetUp AddSingleton<TSingleton>(TSingleton singleton)
            where TSingleton : class
        {
            _services.AddSingleton(typeof(TSingleton));
            return this;
        }

        public IServiceCollectionSetUp AddTransient<TTransient>(TTransient transient) where TTransient : class
        {
            _services.AddTransient(transient.GetType());
            return this;
        }

        public IServiceCollection Create()
            => _services;

        public IServiceCollectionSetUp Remove<TService>()
            where TService : class
        {
            var serviceToRemove = _services.SingleOrDefault(s => s.ServiceType == typeof(TService));
            if (serviceToRemove != null)
                _services.Remove(serviceToRemove);

            return this;
        }

        public IServiceCollectionSetUp SwapScoped<TScoped>(TScoped scoped)
            where TScoped : class
        => Remove<TScoped>()
            .AddScoped(scoped);
        

        public IServiceCollectionSetUp SwapSingleton<TSingleton>(TSingleton singleton)
            where TSingleton : class
        => Remove<TSingleton>()
            .AddSingleton(singleton);

        public IServiceCollectionSetUp SwapTransient<TTransient>(TTransient transient) where TTransient : class
        => Remove<TTransient>()
                .AddTransient(transient);
    }
}
