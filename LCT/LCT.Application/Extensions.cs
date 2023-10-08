using LCT.Core.Entites.Tournaments.Services;
using LCT.Domain.Aggregates.TournamentAggregate.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LCT.Application
{
    public static class Extensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection collection)
        {
            collection.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(Extensions)));
            collection.AddSingleton<ITournamentDomainService, TournamentDomainService>();
            //composition root

            return collection;
        }


    }
}
