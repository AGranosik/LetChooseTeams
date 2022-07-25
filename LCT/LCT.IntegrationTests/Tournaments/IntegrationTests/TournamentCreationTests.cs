using FluentAssertions;
using LCT.Application.Tournaments.Commands;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Infrastructure.EF;
using LCT.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.DFM;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Tournaments.IntegrationTests
{
    [TestFixture]
    public class TournamentCreationTests : Testing<Tournament>
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
            var id = await CreateTournamenCommandHander(request);

            var tournament = await GetTournament(id);
            tournament.Should().NotBeNull();
            tournament.TournamentName.Value.Should().Be(request.Name);
            tournament.Limit.Limit.Should().Be(request.PlayerLimit);
        }

        [Test]
        public async Task Tournament_NameUniqnessChecked_ThrowsAsync()
        {
            //var tournament = Tournament.Create(new Name("unique"), new TournamentLimit(2));
            //await AddAsync(tournament);

            //var action = () => CreateTournamenCommandHander(new CreateTournamentCommand
            //{
            //    Name = "unique",
            //    PlayerLimit = 10
            //});

            //await action.Should().ThrowAsync<DbUpdateException>();
        }

        private async Task<Guid> CreateTournamenCommandHander(CreateTournamentCommand request)
        {
            return await new CreateTournamentCommandHandler(GetRepository()).Handle(request, new CancellationToken());
        }

        private async Task<Tournament> GetTournament(Guid id)
        {
            var repository = GetRepository();
            return await repository.Load(id);
        }

    }
}