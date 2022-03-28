﻿using FluentAssertions;
using LCT.Application.Players.Commands;
using LCT.Application.Tournaments.Hubs;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.Exceptions;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Infrastructure.EF;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.DFM;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Tournaments.IntegrationTests
{
    [TestFixture]
    public class TournamentAssignePlayerTests: Testing<LctDbContext>
    {
        public TournamentAssignePlayerTests()
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
        public async Task AssignPlayer_Success()
        {
            var tournament = await CreateTournament();
            var action = () => AssignPlayerCommandHandleAsync("name", "surname", tournament.Id);
            
            await action.Should().NotThrowAsync();

            var tournamentFromDb = await GetTournamentById(tournament.Id);
            tournamentFromDb.Should().NotBeNull();
            tournament.Players.Count.Should().Be(1);
        }

        [Test]
        public async Task AssignPlayer_playerAlreadyExists_ThrowsExceptionAsync()
        {
            var tournament = await CreateTournament();
            var player = Player.Register(new Name("name"), new Name("surname"));
            tournament.AddPlayer(player);
            await GetDbContext().SaveChangesAsync();

            var action = () => AssignPlayerCommandHandleAsync(player.Name, player.Surname, tournament.Id);

            await action.Should().ThrowAsync<PlayerAlreadyAssignedToTournamentException>();
        }

        private async Task<Guid> AssignPlayerCommandHandleAsync(string name, string surname, Guid tournamentId)
        {
            var hubContext = new Mock<IHubContext<PlayerAssignedHub>>();
            var hubClients = new Mock<IHubClients>();
            var clientProxy = new Mock<IClientProxy>();
            hubClients.Setup(hc => hc.All)
                .Returns(clientProxy.Object);

            hubContext.Setup(hc => hc.Clients)
                .Returns(hubClients.Object);

            return await new AssignPlayerToTournamentCommandHandler(GetDbContext(), hubContext.Object).Handle(new AssignPlayerToTournamentCommand
            {
                Name = name,
                Surname = surname,
                TournamentId = tournamentId
            }, new CancellationToken());
        }

        private async Task<Tournament> GetTournamentById(Guid id)
            => await GetDbContext().Tournaments
            .Include(t => t.Players)
            .SingleOrDefaultAsync(t => t.Id == id);
        private async Task<Tournament> CreateTournament()
        {
            var tournament = Tournament.Create(new Name("test"), new TournamentLimit(3));
            await AddAsync(tournament);
            return tournament;
        }
    }
}
