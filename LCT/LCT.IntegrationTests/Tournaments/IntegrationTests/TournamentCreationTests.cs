using FluentAssertions;
using LCT.Application.Tournaments.Commands;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Exceptions;
using NUnit.DFM;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Tournaments.IntegrationTests
{
    [TestFixture]
    public class TournamentCreationTests : Testing<Tournament>
    {
        public TournamentCreationTests() : base()
        {
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
            await CreateTournamenCommandHander(new CreateTournamentCommand
            {
                Name = "unique",
                PlayerLimit = 10
            });
            var action = () => CreateTournamenCommandHander(new CreateTournamentCommand
            {
                Name = "unique",
                PlayerLimit = 10
            });

            await action.Should().ThrowAsync<TournamentNameNotUniqueException>();
        }

        private async Task<Guid> CreateTournamenCommandHander(CreateTournamentCommand request)
        {
            return await new CreateTournamentCommandHandler(GetRepository(), GetTournamentDomainService()).Handle(request, new CancellationToken());
        }

        private async Task<Tournament> GetTournament(Guid id)
        {
            var repository = GetRepository();
            return await repository.LoadAsync(id);
        }

    }
}