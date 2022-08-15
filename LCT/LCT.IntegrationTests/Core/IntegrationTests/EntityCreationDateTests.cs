using LCT.Core.Aggregates.TournamentAggregate.Entities;
using NUnit.DFM;
using NUnit.Framework;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Core.IntegrationTests
{
    [TestFixture]
    public class EntityCreationDateTests : Testing<Tournament>
    {
        public EntityCreationDateTests() : base()
        {
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
            //var player = Player.Register("sss", "hehe", Guid.NewGuid());
            //(player is Entity).Should().BeTrue();
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
