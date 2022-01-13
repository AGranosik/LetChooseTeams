using LCT.Api.Controllers;
using LCT.Application.Tournaments.Commands;
using LCT.Infrastructure.EF;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.DFM;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LCT.IntegrationTests
{
    [TestFixture]
    public class Tests : Testing<LctDbContext>
    {
        public Tests() : base()
        {
            var configuration = SetUpConfiguration()
                .SetBasePath(Directory.GetCurrentDirectory())
                .SetEnvironment("Development")
                .AddEnvironmentVariables()
                .Create();

            var mocked = new Mock<IMediator>();
            mocked
            .Setup(m => m.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

            var services = SetUpServices()
                .SwapTransient<IMediator>(mocked.Object).Create();

            SetConfiguration(configuration)
            .SetServices(services);
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