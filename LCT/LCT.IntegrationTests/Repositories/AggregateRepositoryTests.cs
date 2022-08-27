using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LCT.Core.Shared.Exceptions;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players;
using NUnit.DFM;
using NUnit.Framework;

namespace LCT.IntegrationTests.Repositories
{
    [TestFixture]
    public class AggregateRepositoryTests : Testing<Tournament>
    {
        public AggregateRepositoryTests()
        {
            this.Environment("Development")
                .ProjectName("LCT.Api")
                .Build();
        }



        [Test]
        public async Task LoadAsync_AggregateDoestNotExist_ThrowsException()
        {
            var repo = GetRepository();
            var func = () => repo.LoadAsync(Guid.Empty);
            await func.Should().ThrowAsync<EntityDoesNotExist>();
        }

        [Test]
        public async Task LoadAsync_FullyCreateAggregate_Success()
        {
            var tournament = await CreateCompleteTournament(6, 6, 6);
            var repo = GetRepository();
            var tournamentFromDb = await repo.LoadAsync(tournament.Id.Value);
            tournamentFromDb.Should().NotBeNull();
        }


        private async Task<Tournament> CreateCompleteTournament(int limit, int players, int selectedTeams)
        {
            var tournament = Tournament.Create("test", limit);

            for (int i = 0; i < players; i++)
            {
                var player = Player.Create(i.ToString(), i.ToString());
                tournament.AddPlayer(player.Name, player.Surname);
            }

            for (int i = 0; i < selectedTeams; i++)
            {
                var player = tournament.Players.ElementAt(i);

                tournament.SelectTeam(player.Name, player.Surname, TournamentTeamNames.Teams[i]);
            }
            await AddAsync(tournament);
            return tournament;
        }
    }
}
