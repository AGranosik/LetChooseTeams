using LCT.Application.Common;
using LCT.Core.Aggregates.TournamentAggregate.Services;
using LCT.Core.Entites.Tournaments.Services;
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
            return collection;
        }
    }
}
