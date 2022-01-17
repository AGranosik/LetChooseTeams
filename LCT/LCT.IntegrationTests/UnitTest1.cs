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

            var mocked = new Mock<IMediator>();
            mocked
            .Setup(m => m.Send(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());
            this.SetBasePath(Directory.GetCurrentDirectory())
                .SetEnvironment("Development")
                .AddEnvironmentVariables();

            this.SwapTransient(mocked.Object);

            this.Environment("Development")
                .ProjectName("LCT.Api")
                .Build();

        }
        [Test]
        public async Task TournamentCreationSuccess()
        {
            var mediator = _scopeFactory.CreateScope().ServiceProvider.GetService<IMediator>();
            var controlelr = new TournamentController(mediator);

            var result = await controlelr.Create(new CreateTournamentCommand()
            {
                Name = "tournamentName",
                PlayerLimit = 10
            });

            
        }
    }
}