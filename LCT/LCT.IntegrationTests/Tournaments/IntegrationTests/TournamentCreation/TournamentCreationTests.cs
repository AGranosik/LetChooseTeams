using FluentAssertions;
using LCT.Application.Tournaments.Commands;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using MongoDB.Driver;
using NUnit.DFM;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Tournaments.IntegrationTests.TournamentCreation
{
    [TestFixture]
    public class TournamentCreationTests : Testing<Tournament>
    {
        public TournamentCreationTests() : base()
        {
            AddTableToTruncate("TournamentStream");
            AddTableToTruncate("Tournament_SetTournamentNameEvent_index");
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
            var command = new CreateTournamentCommand
            {
                Name = "unique",
                PlayerLimit = 10
            };
            await CreateTournamenCommandHander(command);
            var action = () => CreateTournamenCommandHander(command);

            await action.Should().ThrowAsync<MongoServerException>();
        }

        [Test]
        public async Task Tournament_Versioning_Success()
        {
            var id = await CreateTournamenCommandHander(new CreateTournamentCommand
            {
                Name = "unique",
                PlayerLimit = 10
            });

            var tournament = await GetTournament(id);
            tournament.Should().NotBeNull();
            tournament.Version.Should().Be(1);
        }

        private async Task<Guid> CreateTournamenCommandHander(CreateTournamentCommand request)
        {
            return await new CreateTournamentCommandHandler(GetRepository()).Handle(request, new CancellationToken());
        }

        private async Task<Tournament> GetTournament(Guid id)
        {
            var repository = GetRepository();
            return await repository.LoadAsync(id);
        }

    }
}