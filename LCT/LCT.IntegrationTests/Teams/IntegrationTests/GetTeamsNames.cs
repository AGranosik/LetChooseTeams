using FluentAssertions;
using LCT.Application.Teams.Queries;
using LCT.Core.Entities.Tournaments.Types;
using LCT.Infrastructure.EF;
using NUnit.DFM;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Teams.IntegrationTests
{
    [TestFixture]
    public class GetTeamsNames: Testing<LctDbContext>
    {
        public GetTeamsNames() : base()
        {
            this.Environment("Development")
                .ProjectName("LCT.Api")
                .Build();
        }

        [Test]
        public async Task GetTeamsNames_Success()
        {
            var result = await GetTeamsNamesQueryHandler();
            result.Should().NotBeNull();

            result.Should().NotBeEmpty();

            result.Count.Should().Be(TournamentTeamNames.Teams.Count);
            result.SequenceEqual(TournamentTeamNames.Teams);
        }

        private async Task<List<string>> GetTeamsNamesQueryHandler()
            => await new GetTeamsQueryHandler().Handle(new GetTeamsQuery(), new CancellationToken());
    }
}
