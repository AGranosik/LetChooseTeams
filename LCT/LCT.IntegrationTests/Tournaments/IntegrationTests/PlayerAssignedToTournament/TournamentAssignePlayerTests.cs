using FluentAssertions;
using LCT.Application.Players.Commands;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Exceptions;
using LCT.Infrastructure.ClientCommunication.Hubs;
using LCT.IntegrationTests.Mocks;
using MediatR;
using Moq;
using NUnit.DFM;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Tournaments.IntegrationTests.PlayerAssignedToTournament
{
    [TestFixture]
    public class TournamentAssignePlayerTests : Testing<Tournament>
    {
        private Mock<IMediator> _mediatorMock = IMediatorMock.GetMock();
        public TournamentAssignePlayerTests()
        {
            AddTableToTruncate("TournamentStream");
            AddTableToTruncate("Tournament_SetTournamentNameEvent_index");
            SwapSingleton(IHubContextMock.GetMockedHubContext<TournamentHub>());
            SwapSingleton(_mediatorMock.Object);
            Environment("Development")
                .ProjectName("LCT.Api")
                .Build();
        }

        [Test]
        public async Task AssignPlayer_Success()
        {
            var tournament = await CreateTournament();
            var action = () => AssignPlayerCommandHandleAsync("name", "surname", tournament.Id.Value);

            await action.Should().NotThrowAsync();

            var tournamentFromDb = await GetTournamentById(tournament.Id.Value);
            tournamentFromDb.Should().NotBeNull();
            tournamentFromDb.Players.Count.Should().Be(1);
        }

        [Test]
        public async Task AssignPlayer_playerAlreadyExists_ThrowsExceptionAsync()
        {
            var tournament = await CreateTournament();
            tournament.AddPlayer("name", "surname");
            var repository = GetRepository();
            await repository.SaveAsync(tournament);

            var action = () => AssignPlayerCommandHandleAsync("name", "surname", tournament.Id.Value);

            await action.Should().ThrowAsync<PlayerAlreadyAssignedToTournamentException>();
        }



        private async Task AssignPlayerCommandHandleAsync(string name, string surname, Guid tournamentId)
        {
            await new AssignPlayerToTournamentCommandHandler(GetRepository()).Handle(new AssignPlayerToTournamentCommand
            {
                Name = name,
                Surname = surname,
                TournamentId = tournamentId
            }, new CancellationToken());
        }



        private async Task<Tournament> GetTournamentById(Guid id)
            => await GetRepository().LoadAsync(id);
        private async Task<Tournament> CreateTournament()
        {
            var tournament = Tournament.Create("test", 3);
            await SaveAsync(tournament);
            return tournament;
        }
    }
}
