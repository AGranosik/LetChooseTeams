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
    public class SetTournamentNameTests : Testing<Tournament>
    {
        public SetTournamentNameTests()
        {
            AddTableToTruncate("TournamentStream");
            AddTableToTruncate("Tournament_SetTournamentNameEvent_index");
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
            var dbTournament = await GetTournamentById(tournament.Id.Value);
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

        [Test]
        public async Task SetName_SecondUpdateToTheFirstName_Success()
        {
            var tournament = await CreateTournament();
            var oldName = tournament.TournamentName.Value;
            var newName = oldName + oldName;
            await SetTournamentName(new SetTournamentNameCommand
            {
                Name = newName,
                TournamentId = tournament.Id.Value
            });

            var dbTournament = await GetTournamentById(tournament.Id.Value);
            dbTournament.TournamentName.Value.Should().Be(newName);

            var func = () => SetTournamentName(new SetTournamentNameCommand
            {
                Name = oldName,
                TournamentId = tournament.Id.Value
            });

            await func.Should().NotThrowAsync();


            dbTournament = await GetTournamentById(tournament.Id.Value);
            dbTournament.TournamentName.Value.Should().Be(oldName);
        }

        [Test]
        public async Task SetName_Multiple_VersionedCorrectly()
        {
            var tournament = await CreateTournament();
            var repo = GetRepository();

            tournament.SetName("hehehe");
            tournament.SetName("hehehe2222");

            var func = () => repo.SaveAsync(tournament, tournament.Version);
            await func.Should().NotThrowAsync();
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
