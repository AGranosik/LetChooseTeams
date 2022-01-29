using FluentAssertions;
using LCT.Api.Controllers;
using LCT.Application.Tournaments.Commands;
using LCT.Core.Entites.Tournament.Entities;
using LCT.Core.Entites.Tournament.ValueObjects;
using LCT.Infrastructure.EF;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.DFM;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Tournaments.IntegrationTests
{
    [TestFixture]
    public class Tests : Testing<LctDbContext>
    {
        public Tests() : base()
        {
            AddTablesToTruncate(new List<string>
            {
                nameof(LctDbContext.Players),
                nameof(LctDbContext.Tournaments)
            });

            this.SetBasePath(Directory.GetCurrentDirectory())
                .SetEnvironment("Development")
                .AddEnvironmentVariables();
            this.Environment("Development")
                .ProjectName("LCT.Api")
                .Build();

        }
        [Test]
        public async Task TournamentCreationSuccess()
        {
            var result = await CreateTournamentApi(new CreateTournamentCommand
            {
                Name = "tournamentName",
                PlayerLimit = 10
            });

            result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public async Task Tournament_NameUniqnessChecked_ThrowsAsync()
        {
            var tournament = Tournament.Create(new Name("unique"), new TournamentLimit(2));
            await AddAsync(tournament);

            var result = await CreateTournamentApi(new CreateTournamentCommand
            {
                Name = "unique",
                PlayerLimit = 10
            });

            result?.Should().BeOfType<BadRequestObjectResult>();
        }

        private Task<IActionResult> CreateTournamentApi(CreateTournamentCommand request)
        {
            var mediator = _scope.ServiceProvider.GetService<IMediator>();
            return new TournamentController(mediator).Create(request);
        }

    }
}