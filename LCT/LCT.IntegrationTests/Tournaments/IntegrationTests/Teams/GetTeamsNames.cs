using FluentAssertions;
using LCT.Application.Teams.Queries;
using LCT.Core.Shared.Exceptions;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players;
using NUnit.DFM;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Tournaments.IntegrationTests.Teams
{
    [TestFixture]
    public class GetTeamsNames : Testing<Tournament>
    {
        public GetTeamsNames() : base()
        {
            Environment("Development")
                .ProjectName("LCT.Api")
                .Build();
        }

        [Test]
        public async Task GetTeamsNames_AnyTeamSelected()
        {
            var tournament = await CreateTournamentWithPlayers(0);
            var result = await GetTeamsNamesQueryHandler(tournament.Id.Value);
            result.Should().NotBeNull();

            result.Should().NotBeEmpty();

            result.Count.Should().Be(TournamentTeamNames.Teams.Count);

            result.All(r => TournamentTeamNames.Teams.Any(t => t == r.Name && !r.Selected)).Should().BeTrue();
        }

        [Test]
        public async Task GetTeamsNames_SomeTeamSelected()
        {
            var tournament = await CreateTournamentWithPlayers(3);
            var result = await GetTeamsNamesQueryHandler(tournament.Id.Value);

            result.Should().NotBeNull();
            result.Should().NotBeEmpty();

            result.Count.Should().Be(TournamentTeamNames.Teams.Count);

            result.Count(r => r.Selected).Should().Be(3);
        }

        private async Task<Tournament> CreateTournamentWithPlayers(int numberOfPLayers)
        {
            var tournament = Tournament.Create("hehe", numberOfPLayers < 2 ? 3 : numberOfPLayers);
            for (int i = 0; i < numberOfPLayers; i++)
            {
                var player = Player.Create(i.ToString(), i.ToString());
                tournament.AddPlayer(player.Name, player.Surname);
                tournament.SelectTeam(player.Name, player.Surname, TournamentTeamNames.Teams[i]);
            }

            await AddAsync(tournament);
            return tournament;
        }

        private async Task<List<TeamToSelectDto>> GetTeamsNamesQueryHandler(Guid tournamentId)
            => await new GetTeamsQueryHandler(GetRepository()).Handle(new GetTeamsQuery() { TournamentId = tournamentId }, new CancellationToken());
    }
}
