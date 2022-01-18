using Microsoft.Extensions.DependencyInjection;
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
        IServiceCollectionSetUp SwapScoped<TType, TObject>(TObject scoped)
            where TObject : class, TType;

        IServiceCollectionSetUp AddTransient<TTransient>(TTransient transient)
            where TTransient : class;
        IServiceCollectionSetUp SwapTransient<TTransient>(TTransient transient)
            where TTransient : class;

        IServiceCollection Create(IServiceCollection services);
    }
}
