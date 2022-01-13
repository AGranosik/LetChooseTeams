using LCT.Api.Controllers;
using LCT.Application.Tournaments.Commands;
using LCT.Infrastructure.EF;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NUnit.DFM;
using NUnit.Framework;
using System.IO;
using System.Threading.Tasks;

namespace LCT.IntegrationTests
{
    [TestFixture]
    public class Tests : Testing<LctDbContext>
    {
        public Tests() : base()
        {
            var configuration = Configure()
                .SetBasePath(Directory.GetCurrentDirectory())
                .SetEnvironment("Development")
                .AddEnvironmentVariables()
                .Build();

            SetConfiguration(configuration);
        }
        [Test]
        public async Task TournamentCreationSuccess()
        {
            var mediator = _scopeFactory.CreateScope().ServiceProvider.GetService<IMediator>();
            var controlelr = new TournamentController(mediator);

            await controlelr.Create(new CreateTournamentCommand()
            {
                Name = "tournamentName",
                PlayerLimit = 10
            });
        }
    }
}