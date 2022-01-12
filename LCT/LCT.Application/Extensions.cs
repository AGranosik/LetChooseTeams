using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace LCT.Application
{
    public static class Extensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection collection)
        {
            collection.AddMediatR(typeof(Extensions));
            return collection;
        }
    }
}
