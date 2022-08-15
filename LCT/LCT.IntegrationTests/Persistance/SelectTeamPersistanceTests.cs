using LCT.Core.Aggregates.TournamentAggregate.Entities;
using NUnit.DFM;
using NUnit.Framework;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Persistance
{
    [TestFixture]
    public class SelectTeamPersistanceTests: Testing<Tournament>
    {
        public SelectTeamPersistanceTests()
        {
            this.Environment("Development")
            .ProjectName("LCT.Api")
            .Build();
        }

        [Test]
        public async Task PlayerCannotSelectTeamTwice()
        {
            //var tournament = CreateTournament();
            //var dbContext = GetRepository();
            //await dbContext.Tournaments.AddAsync(tournament);
            //await dbContext.SaveChangesAsync();
            //dbContext.ChangeTracker.Clear();
            //var tournamentFromDb = await dbContext.Tournaments
            //    .Include(t => t.Players)
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync(t => t.Id == tournament.Id);

            //tournamentFromDb.SelectTeam(tournament.Players.First().Id, TournamentTeamNames.Teams.First());
            //dbContext.Update(tournamentFromDb);
            //var func = () => dbContext.SaveChangesAsync();
            //await func.Should().ThrowAsync<DbUpdateException>();
        }

        [Test]
        public async Task TeamCannotSelectTeamTwice()
        {
            //var tournament = CreateTournament();
            //var dbContext = GetRepository();
            //await dbContext.Tournaments.AddAsync(tournament);
            //await dbContext.SaveChangesAsync();
            //dbContext.ChangeTracker.Clear();
            //var tournamentFromDb = await dbContext.Tournaments
            //    .Include(t => t.Players)
            //    .AsNoTracking()
            //    .FirstOrDefaultAsync(t => t.Id == tournament.Id);

            //tournamentFromDb.SelectTeam(tournament.Players.Last().Id, TournamentTeamNames.Teams.Last());
            //dbContext.Update(tournamentFromDb);
            //var func = () => dbContext.SaveChangesAsync();
            //await func.Should().ThrowAsync<DbUpdateException>();
        }

        private Tournament CreateTournament()
        {
            return null;
            //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(3));
            //var players = new List<Player>()
            //{
            //    Player.Register(new Name("1"), new Name("2")),
            //    Player.Register(new Name("3"), new Name("4"))
            //};
            //tournament.AddPlayer(players[0]);
            //tournament.AddPlayer(players[1]);
            //tournament.SelectTeam(players[0].Id, TournamentTeamNames.Teams.Last());
            //return tournament;
        }
    }
}
