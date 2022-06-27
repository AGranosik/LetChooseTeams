using FluentAssertions;
using LCT.Application.Tournaments.Queries;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.Exceptions;
using LCT.Core.Entites.Tournaments.Services;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Core.Entities.Tournaments.Types;
using LCT.Infrastructure.EF;
using Microsoft.EntityFrameworkCore;
using NUnit.DFM;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Tournaments.IntegrationTests
{
    [TestFixture]
    public class TournamentDrawTeamTests : Testing<LctDbContext>
    {
        public TournamentDrawTeamTests() : base()
        {
            AddTablesToTruncate(new List<string>
            {
                nameof(LctDbContext.DrawnTeams),
                nameof(LctDbContext.SelectedTeams),
                nameof(LctDbContext.Players),
                nameof(LctDbContext.Tournaments)
            });

            this.Environment("Development")
                .ProjectName("LCT.Api")
                .Build();
        }

        [Test]
        public async Task TournamentDoesNotExist_ThrowsException()
        {
            var action = () => DrawTeamQueryHandlerResult(new DrawTeamForPlayersQuery { TournamentId = Guid.Empty });

            await action.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task NotAllPlayersPicked_ThrowsException()
        {
            var tournament = await CreateCompleteTournament(3, 3, 2);
            var action = () => DrawTeamQueryHandlerResult(new DrawTeamForPlayersQuery { TournamentId = tournament.Id });

            await action.Should().ThrowAsync<NotAllPlayersSelectedTeamException>();
        }

        [Test]
        public async Task NotAllPlayersRegistered_ThrowsException()
        {
            var tournament = await CreateCompleteTournament(3, 2, 2);
            var action = () => DrawTeamQueryHandlerResult(new DrawTeamForPlayersQuery { TournamentId = tournament.Id });

            await action.Should().ThrowAsync<NotAllPlayersRegisteredException>();
        }

        [Test]
        public async Task TeamsDrawnProperly_NoException()
        {
            var tournament = await CreateCompleteTournament(3, 3, 3);
            var action = () => DrawTeamQueryHandlerResult(new DrawTeamForPlayersQuery { TournamentId = tournament.Id });

            await action.Should().NotThrowAsync();
        }

        [Test]
        public async Task TeamsDrawnProperly_DataSavedInDb()
        {
            var tournament = await CreateCompleteTournament(7, 7, 7);
            await DrawTeamQueryHandlerResult(new DrawTeamForPlayersQuery { TournamentId = tournament.Id });

            var tournamentDb = await GetTournamentByIdAsync(tournament.Id);
            var drawTeams = tournamentDb.DrawTeams;
            drawTeams.Should().NotBeEmpty();
            drawTeams.Count.Should().Be(7);

        }

        private async Task<Tournament> GetTournamentByIdAsync(Guid tournamentId)
        {
            var context = GetDbContext();
            return await context.Tournaments
                .Include(t => t.Players)
                .Include(t => t.SelectedTeams)
                .Include(t => t.DrawTeams)
                .FirstOrDefaultAsync(t => t.Id == tournamentId);
        }

        private async Task<Tournament> CreateCompleteTournament(int limit, int players, int selectedTeams)
        {
            return null;
            //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(limit));
            //for(int i = 0; i < players; i++)
            //{
            //    var player = Player.Register(new Name(i.ToString()), new Name(i.ToString()));
            //    player.Id = Guid.NewGuid();
            //    tournament.AddPlayer(player);
            //}

            //for(int i =0; i < selectedTeams; i++)
            //{
            //    var player = tournament.Players.ElementAt(i);

            //    tournament.SelectTeam(player.Id, TournamentTeamNames.Teams[i]);
            //}

            //var dbContext = GetDbContext();
            //await dbContext.Tournaments.AddAsync(tournament);
            //await dbContext.SaveChangesAsync();

            //return tournament;
        }

        private async Task<List<DrawnTeamDto>> DrawTeamQueryHandlerResult(DrawTeamForPlayersQuery query)
            => await new DrawTeamForPlayersQueryHandler(GetDbContext(), new TournamentDomainService()).Handle(query, CancellationToken.None);
        
    }
}
