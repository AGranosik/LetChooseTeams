using FluentAssertions;
using LCT.Api.Controllers;
using LCT.Application.Tournaments.Commands;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Infrastructure.EF;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.DFM;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var request = new CreateTournamentCommand
            {
                Name = "testName",
                PlayerLimit = 10
            };
            var result = await CreateTournamentApi(request);

            result.Should().BeOfType<OkObjectResult>();

            var tournaments = await GetTournaments();
            tournaments.Should().NotBeEmpty();
            tournaments.Count.Should().Be(1);

            var tournament = tournaments.First();
            tournament.TournamentName.Value.Should().Be(request.Name);
            tournament.Limit.Limit.Should().Be(request.PlayerLimit);
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

            var tournaments = await GetTournaments();
            tournaments.Should().BeEmpty();
        }

        private Task<IActionResult> CreateTournamentApi(CreateTournamentCommand request)
        {
            var mediator = _scope.ServiceProvider.GetService<IMediator>();
            return new TournamentController(mediator).Create(request);
        }

        private Task<List<Tournament>> GetTournaments()
        {
            var dbContext = GetDbContext();
            return dbContext.Tournaments.ToListAsync();
        }

    }
}