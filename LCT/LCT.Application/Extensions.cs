using LCT.Application.Common;
using LCT.Application.Common.Configs;
using LCT.Core.Entites.Tournaments.Services;
using LCT.Domain.Aggregates.TournamentAggregate.Services;
using LCT.Infrastructure;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace LCT.Application
{
    public static class Extensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection collection)
        {
            collection.AddMediatR(typeof(Extensions));
            collection.AddSingleton<IQRCodeCreator, QRCodeCreator>();
            collection.AddSingleton<ITournamentDomainService, TournamentDomainService>();

            collection.ConfigureFrontendUrl();

            return collection;
        }

        private static IServiceCollection ConfigureFrontendUrl(this IServiceCollection services)
        {
            var fe = services.GetOptions<FrontendConfiguration>("fe");
            services.AddSingleton(fe);

            return services;
        }
    }
}
