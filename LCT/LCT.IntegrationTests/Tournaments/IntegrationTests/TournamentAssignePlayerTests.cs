using FluentAssertions;
using LCT.Api.Controllers;
using LCT.Application.Players.Commands;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Infrastructure.EF;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.DFM;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
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
            var result = await AssignPlayerApiAsync("name", "surname", tournament.Id);
            
            result.Should().BeOfType<OkObjectResult>();

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

            var result = await AssignPlayerApiAsync(player.Name, player.Surname, tournament.Id);
            
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        private async Task<IActionResult> AssignPlayerApiAsync(string name, string surname, Guid tournamentId)
        {
            var mediator = _scope.ServiceProvider.GetService<IMediator>();
            return await new TournamentController(mediator).AssignPlayerToTournament(new AssignPlayerToTournamentCommand
            {
                Name = name,
                Surname = surname,
                TournamentId = tournamentId
            });
        }

        private Task<Tournament> GetTournamentById(Guid id)
            => GetDbContext().Tournaments
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
