using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NUnit.DFM.Interfaces;

namespace NUnit.DFM.Builders
{
    public class DfmServiceCollectionBuilder: IServiceCollectionSetUp
    {
        private readonly List<Type> _singletonsToAdd = new List<Type>();
        private readonly List<Type> _transientsToAdd = new List<Type>();
        private readonly List<Type> _scopedToAdd = new List<Type>();
        private readonly List<Type> _serviesToRemove = new List<Type>();

        public DfmServiceCollectionBuilder()
        {
        }

        public IServiceCollectionSetUp AddScoped<TScoped>(TScoped scoped)
            where TScoped : class
        {
            _scopedToAdd.Add(typeof(TScoped));
            return this;
        }

        public IServiceCollectionSetUp AddSingleton<TSingleton>(TSingleton singleton)
            where TSingleton : class
        {
            _singletonsToAdd.Add(typeof (TSingleton));
            return this;
        }

        public IServiceCollectionSetUp AddTransient<TTransient>(TTransient transient) where TTransient : class
        {
            _transientsToAdd.Add(typeof (TTransient));
            return this;
        }

        public IServiceCollection Create(IServiceCollection services)
        {
            var servicesToRemove = services.Where(s => _serviesToRemove.Any(r => s.ServiceType == r)).ToList();
            foreach (var service in servicesToRemove)
                services.Remove(service);

            foreach(var service in _singletonsToAdd)
                services.AddSingleton(service);
            foreach (var service in _transientsToAdd)
                services.AddTransient(service);
            foreach (var service in _scopedToAdd)
                services.AddScoped(service);

            return services;
        }

        public IServiceCollectionSetUp Remove<TService>()
            where TService : class
        {
            _serviesToRemove.Add(typeof(TService));
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
