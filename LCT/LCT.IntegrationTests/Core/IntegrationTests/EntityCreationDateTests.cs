using FluentAssertions;
using LCT.Core.Entites;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using NUnit.DFM;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Core.IntegrationTests
{
    [TestFixture]
    public class EntityCreationDateTests : Testing<LctDbContext>
    {
        public EntityCreationDateTests() : base()
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
        public async Task TournamentIsEntityType()
        {
            //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            //(tournament is Entity).Should().BeTrue();
        }

        [Test]
        public async Task TournamentWithCreationDate_Success()
        {
            //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            //var dbContext = GetDbContext();
            //dbContext.Tournaments.Add(tournament);
            //await dbContext.SaveChangesAsync();

            //var dbTournament = await dbContext.Tournaments.FirstOrDefaultAsync(t => t.Id == tournament.Id);
            //dbTournament.CreatedAt.Should().NotBe(default(DateTime));
        }

        [Test]
        public void PlayerIsEntityType()
        {
            var player = Player.Register(new Name("sss"), new Name("hehe"));
            (player is Entity).Should().BeTrue();
        }

        [Test]
        public async Task PlayerWithCreationDate_Success()
        {
            //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            //tournament.AddPlayer(Player.Register(new Name("sss"), new Name("hehe")));
            //var dbContext = GetDbContext();
            //dbContext.Tournaments.Add(tournament);
            //await dbContext.SaveChangesAsync();

            //var dbTournament = await dbContext.Tournaments.FirstOrDefaultAsync(t => t.Id == tournament.Id);
            //dbTournament.Players.Count.Should().Be(1);
            //dbTournament.Players.All(p => p.CreatedAt != default(DateTime)).Should().BeTrue();
        }
    }
}
