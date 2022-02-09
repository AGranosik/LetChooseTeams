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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Tournaments.IntegrationTests
{
    [TestFixture]
    public class TournamentCreationTests : Testing<LctDbContext>
    {
        public TournamentCreationTests() : base()
        {
            AddTablesToTruncate(new List<string>
            {
                nameof(LctDbContext.Players),
                nameof(LctDbContext.Tournaments)
            });

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
            var action = () => CreateTournamenCommandHander(request);

            action.Should().NotThrowAsync();

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

            var action = () => CreateTournamenCommandHander(new CreateTournamentCommand
            {
                Name = "unique",
                PlayerLimit = 10
            });

            await action.Should().ThrowAsync<DbUpdateException>();
        }

        private async Task<Unit> CreateTournamenCommandHander(CreateTournamentCommand request)
        {
            return await new CreateTournamentCommandHandler(GetDbContext()).Handle(request, new CancellationToken());
        }

        private Task<List<Tournament>> GetTournaments()
        {
            var dbContext = GetDbContext();
            return dbContext.Tournaments.ToListAsync();
        }

    }
}