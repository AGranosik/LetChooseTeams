using FluentAssertions;
using LCT.Application.Teams.Queries;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Core.Entities.Tournaments.Types;
using LCT.Core.Shared.Exceptions;
using LCT.Infrastructure.EF;
using NUnit.DFM;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Teams.IntegrationTests
{
    [TestFixture]
    public class GetTeamsNames: Testing<LctDbContext>
    {
        public GetTeamsNames() : base()
        {
            this.Environment("Development")
                .ProjectName("LCT.Api")
                .Build();
        }

        [Test]
        public async Task GetTeamsNames_TournamentDoesNotExist_DoesNotThrowException()
        {
            var func = async () => await GetTeamsNamesQueryHandler(Guid.NewGuid());
            await func.Should().NotThrowAsync<EntityDoesNotExist>();
        }

        [Test]
        public async Task GetTeamsNames_AnyTeamSelected()
        {
            var tournament = await CreateTournamentWithPlayers(0);
            var result = await GetTeamsNamesQueryHandler(tournament.Id);
            result.Should().NotBeNull();

            result.Should().NotBeEmpty();

            result.Count.Should().Be(TournamentTeamNames.Teams.Count);

            result.All(r => TournamentTeamNames.Teams.Any(t => t == r.Name && !r.Selected)).Should().BeTrue();
        }

        [Test]
        public async Task GetTeamsNames_SomeTeamSelected()
        {
            var tournament = await CreateTournamentWithPlayers(3);
            var result = await GetTeamsNamesQueryHandler(tournament.Id);
            result.Should().NotBeNull();

            result.Should().NotBeEmpty();

            result.Count.Should().Be(TournamentTeamNames.Teams.Count);

            result.Count(r => r.Selected).Should().Be(3);
        }

        private async Task<Tournament> CreateTournamentWithPlayers(int numberOfPLayers)
        {
            return null;
            //var tournament = Tournament.Create(new Name("hehe"), new TournamentLimit(numberOfPLayers < 2 ? 3 : numberOfPLayers));
            //for(int i = 0; i< numberOfPLayers; i++)
            //{
            //    var player = Player.Register(new Name(i.ToString()), new Name(i.ToString()));
            //    player.Id = Guid.NewGuid();
            //    tournament.AddPlayer(player);
            //    tournament.SelectTeam(player.Id, TournamentTeamNames.Teams[i]);
            //}
            //var dbContext = GetDbContext();
            //await dbContext.Tournaments.AddAsync(tournament);
            //await dbContext.SaveChangesAsync();

            //return tournament;
        }

        private async Task<List<TeamToSelectDto>> GetTeamsNamesQueryHandler(Guid tournamentId)
            => await new GetTeamsQueryHandler(GetDbContext()).Handle(new GetTeamsQuery() { TournamentId = tournamentId }, new CancellationToken());
    }
}
