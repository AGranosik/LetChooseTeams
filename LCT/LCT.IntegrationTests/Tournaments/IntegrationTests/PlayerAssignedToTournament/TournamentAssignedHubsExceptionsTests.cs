using System.Threading;
using System;
using System.Threading.Tasks;
using LCT.Application.Players.Commands;
using LCT.Application.Tournaments.Hubs;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.IntegrationTests.Mocks;
using MediatR;
using Moq;
using NUnit.DFM;
using NUnit.Framework;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;

namespace LCT.IntegrationTests.Tournaments.IntegrationTests.PlayerAssignedToTournament
{
    [TestFixture]
    public class TournamentAssignedHubsExceptionsTests : Testing<Tournament>
    {
        private Mock<IHubContext<TournamentHub>> _hubContextMock = new Mock<IHubContext<TournamentHub>>();
        public TournamentAssignedHubsExceptionsTests()
        {
            _hubContextMock.Setup(h => h.Clients)
                .Throws<Exception>();

            SwapSingleton(_hubContextMock.Object);
            Environment("Development")
                .ProjectName("LCT.Api")
                .Build();
        }


        [Test]
        public async Task AssignPlayer_ReturnsPlayerIdDespiteHubException()
        {
            var tournament = await CreateTournament();
            var result = await AssignPlayerCommandHandleAsync("name", "surname", tournament.Id.Value);

            result.Should().NotBeNull();
        }

        private async Task<Unit> AssignPlayerCommandHandleAsync(string name, string surname, Guid tournamentId)
        {
            return await new AssignPlayerToTournamentCommandHandler(GetRepository()).Handle(new AssignPlayerToTournamentCommand
            {
                Name = name,
                Surname = surname,
                TournamentId = tournamentId
            }, new CancellationToken());
        }

        private async Task<Tournament> CreateTournament()
        {
            var tournament = Tournament.Create("test", 3);
            await AddAsync(tournament);
            return tournament;
        }
    }
}
