using FluentAssertions;
using LCT.Application.Players.Commands;
using LCT.Application.Tournaments.Hubs;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Events;
using LCT.Domain.Aggregates.TournamentAggregate.Exceptions;
using LCT.IntegrationTests.Mocks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using NUnit.DFM;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Tournaments.IntegrationTests
{
    [TestFixture]
    public class TournamentAssignePlayerTests: Testing<Tournament>
    {
        public TournamentAssignePlayerTests()
        {
            SwapSingleton<IHubContext<TournamentHub>>(IHubContextMock.GetMockedHubContext<TournamentHub>());
            this.Environment("Development")
                .ProjectName("LCT.Api")
                .Build();
        }

        [Test]
        public async Task AssignPlayer_Success()
        {
            var tournament = await CreateTournament();
            var action = () => AssignPlayerCommandHandleAsync("name", "surname", tournament.Id.Value, IMediatorMock.GetMock());

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

            var action = () => AssignPlayerCommandHandleAsync("name", "surname", tournament.Id.Value, IMediatorMock.GetMock());

            await action.Should().ThrowAsync<PlayerAlreadyAssignedToTournamentException>();
        }

        [Test]
        public async Task AssignPlayer_ReturnsPlayerIdDespiteHubException()
        {
            var tournament = await CreateTournament();
            var result = await AssignPlayerCommandHandleAsync("name", "surname", tournament.Id.Value, IMediatorMock.GetMockWithException<PlayerAddedDomainEvent>());

            result.Should().NotBeNull();

        }

        private async Task<Unit> AssignPlayerCommandHandleAsync(string name, string surname, Guid tournamentId, IMediator mediator)
        {
            return await new AssignPlayerToTournamentCommandHandler(mediator, GetRepository()).Handle(new AssignPlayerToTournamentCommand
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
            await AddAsync(tournament);
            return tournament;
        }
    }
}
