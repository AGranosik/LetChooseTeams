using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LCT.Application.Tournaments.Commands;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using MediatR;
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
            var newName = "hehe";
            tournament.Version.Should().Be(1);
            await SetTournamentName(new SetTournamentNameCommand
            {
                Name = newName,
                TournamentId = tournament.Id.Value
            });


            var dbTournament = await GetTournamentById(tournament.Id.Value);
            dbTournament.Should().NotBeNull();
            dbTournament.Version.Should().Be(2);
        }

        [Test]
        public async Task SetName_UpdatedBefore_ThrowsException()
        {
            var notSavedName = "not sabved name";
            var tournament = await CreateTournament();
            var dbTournament = await GetTournamentById(tournament.Id.Value); //same versions...
            var newName = "hehe";
            await SetTournamentName(new SetTournamentNameCommand
            {
                Name = newName,
                TournamentId = tournament.Id.Value
            });

            dbTournament.SetName(notSavedName);
            var func = () => SaveAsync(dbTournament, dbTournament.Version);

            await func.Should().ThrowAsync<Exception>();
            dbTournament = await GetTournamentById(tournament.Id.Value);
            dbTournament.Version.Should().Be(2);
            dbTournament.TournamentName.Value.Should().Be(newName);

        }

        private async Task<Tournament> CreateTournament()
        {
            var tournament = Tournament.Create("name", 2);
            await SaveAsync(tournament);
            return tournament;
        }

        private async Task<Unit> SetTournamentName(SetTournamentNameCommand request)
            => await new SetTournamentNameCommandHandler(GetRepository()).Handle(request, CancellationToken.None);

        private async Task<Tournament> GetTournamentById(Guid id)
            => await GetRepository().LoadAsync(id);
    }
}
