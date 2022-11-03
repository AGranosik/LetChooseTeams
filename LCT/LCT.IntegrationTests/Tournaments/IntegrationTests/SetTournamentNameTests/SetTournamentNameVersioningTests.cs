using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using NUnit.DFM;
using NUnit.Framework;

namespace LCT.IntegrationTests.Tournaments.IntegrationTests.SetTournamentNameTests
{
    [TestFixture]
    public class SetTournamentNameVersioningTests : Testing<Tournament>
    {
        public SetTournamentNameVersioningTests()
        {
            this.Environment("Development")
                .ProjectName("LCT.Api")
                .Build();

        }

        [Test]
        public async Task SetName_Versioned_Success()
        {
            var tournament = await CreateTournament();
            tournament.SetName("new name");

            await SaveAsync(tournament);
            tournament.Version.Should().Be(2);

            var dbTournament = await GetTournamentById(tournament.Id.Value);
            dbTournament.Should().NotBeNull();
            dbTournament.Version.Should().Be(2);
        }

        private async Task<Tournament> CreateTournament()
        {
            var tournament = Tournament.Create("name", 2);
            await SaveAsync(tournament);
            return tournament;
        }

        private async Task<Tournament> GetTournamentById(Guid id)
            => await GetRepository().LoadAsync(id);
    }
}
