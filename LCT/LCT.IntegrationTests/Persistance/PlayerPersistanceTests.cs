using FluentAssertions;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Infrastructure.EF;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NUnit.DFM;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Persistance
{
    [TestFixture]
    public class PlayerPersistanceTests: Testing<Tournament>
    {

        public PlayerPersistanceTests()
        {
            AddTableToTruncate(nameof(LctDbContext.Players));
            AddTableToTruncate(nameof(LctDbContext.Tournaments));

            this.Environment("Development")
            .ProjectName("LCT.Api")
            .Build();
        }

        [Test]
        public async Task PlayerCanBeAdded()
        {
            //var player = Player.Register(new Name("test"), new Name("hehe"));
            //var dbContext = GetRepository();
            //dbContext.Players.Add(player);
            //var func = () => dbContext.SaveChangesAsync();
            //await func.Should().NotThrowAsync();
        }

        [Test]
        public async Task CannotAssignToSamePlayersNameToASingleTournament()
        {
            //var player = Player.Register(new Name("test"), new Name("hehe"));
            //var dbContext = GetDbContext();
            //var tournament = Tournament.Create(new Name("hehe"), new TournamentLimit(3));
            //tournament.AddPlayer(player);
            //dbContext.Tournaments.Add(tournament);
            //await dbContext.SaveChangesAsync();

            //dbContext.ChangeTracker.Clear();
            //var duplicatePlayer = Player.Register(new Name("test"), new Name("hehe"));
            //var id = tournament.Id;
            //var tournamentFromDb = await dbContext.Tournaments.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            //tournamentFromDb.AddPlayer(duplicatePlayer);
            //dbContext.Tournaments.Update(tournamentFromDb);
            //var func = () => dbContext.SaveChangesAsync();
            //await func.Should().ThrowAsync<DbUpdateException>();
        }

        [Test]
        public async Task CanAssignPlayerWithDiffrentNameToASingleTournament()
        {
            //var player = Player.Register(new Name("test"), new Name("hehe"));
            //var dbContext = GetDbContext();
            //var tournament = Tournament.Create(new Name("hehe"), new TournamentLimit(3));
            //tournament.AddPlayer(player);
            //dbContext.Tournaments.Add(tournament);
            //await dbContext.SaveChangesAsync();

            //dbContext.ChangeTracker.Clear();
            //var duplicatePlayer = Player.Register(new Name("test2"), new Name("hehe"));
            //var id = tournament.Id;
            //var tournamentFromDb = await dbContext.Tournaments.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            //tournamentFromDb.AddPlayer(duplicatePlayer);
            //dbContext.Tournaments.Update(tournamentFromDb);
            //var func = () => dbContext.SaveChangesAsync();
            //await func.Should().NotThrowAsync();
        }

        [Test]
        public async Task CanAssignPlayerWithDiffrentSurnameToASingleTournament()
        {
            //var player = Player.Register(new Name("test"), new Name("hehe"));
            //var dbContext = GetDbContext();
            //var tournament = Tournament.Create(new Name("hehe"), new TournamentLimit(3));
            //tournament.AddPlayer(player);
            //dbContext.Tournaments.Add(tournament);
            //await dbContext.SaveChangesAsync();

            //dbContext.ChangeTracker.Clear();
            //var duplicatePlayer = Player.Register(new Name("test"), new Name("hehe2"));
            //var id = tournament.Id;
            //var tournamentFromDb = await dbContext.Tournaments.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            //tournamentFromDb.AddPlayer(duplicatePlayer);
            //dbContext.Tournaments.Update(tournamentFromDb);
            //var func = () => dbContext.SaveChangesAsync();
            //await func.Should().NotThrowAsync();
        }
    }
}
