using FluentAssertions;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using NUnit.DFM;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Persistance
{
    [TestFixture]
    public class PlayerPersistanceTests: Testing<LctDbContext>
    {

        public PlayerPersistanceTests()
        {
            AddTableToTruncate(nameof(LctDbContext.Players));

            this.Environment("Development")
            .ProjectName("LCT.Api")
            .Build();
        }

        [Test]
        public async Task PlayerCanBeAdded()
        {
            var player = Player.Register(new Name("test"), new Name("hehe"));
            var dbContext = GetDbContext();
            dbContext.Players.Add(player);
            var func = () => dbContext.SaveChangesAsync();
            await func.Should().NotThrowAsync();
        }

        [Test]
        public async Task CannotAssignToSamePlayersNameToASingleTournament()
        {
            var player = Player.Register(new Name("test"), new Name("hehe"));
            var dbContext = GetDbContext();
            dbContext.Players.Add(player);
            await dbContext.SaveChangesAsync();

            var tournament = Tournament.Create(new Name("hehe"), new TournamentLimit(3));
            tournament.AddPlayer(player);
            dbContext.Tournaments.Add(tournament);
            await dbContext.SaveChangesAsync();
            var duplicatePlayer = Player.Register(new Name("test"), new Name("hehe"));
            var id = tournament.Id;
            tournament = null;
            var tournamentFromDb = await dbContext.Tournaments.FirstOrDefaultAsync(t => t.Id == id); // avoid eager loading
            tournamentFromDb.AddPlayer(player);
            var func = () => dbContext.SaveChangesAsync();
            await func.Should().NotThrowAsync();

        }
    }
}
