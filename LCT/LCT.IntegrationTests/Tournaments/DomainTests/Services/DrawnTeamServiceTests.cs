using FluentAssertions;
using LCT.Application.Common.Interfaces;
using LCT.Core.Entites.Tournaments.Services;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams;
using LCT.Infrastructure.Persistance.Mongo;
using Microsoft.Extensions.DependencyInjection;
using NUnit.DFM;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace LCT.IntegrationTests.Tournaments.DomainTests.Services
{
    [TestFixture]
    public class DrawnTeamServiceTests : Testing<Tournament>
    {
        public DrawnTeamServiceTests() : base()
        {
            this.Environment("Development")
                .ProjectName("LCT.Api")
                .Build();
        }
        [Test]
        public void TeamsCannotBeTheSameAsSelected()
        {
            const int numberOfPlayers = 10;

            var teams = new List<SelectedTeam>();
            var players = CreatePlayer(numberOfPlayers);
            for(int i = 0; i < numberOfPlayers; i++)
            {
                teams.Add(SelectedTeam.Create(players[i], TournamentTeamNames.Teams[i]));
            }

            var result = new TournamentDomainService(GetPersistanceClient()).DrawTeamForPlayers(teams);
            result.Should().NotBeNullOrEmpty();
        }

        [Test]
        public void NumberOfDrawnTeamSameAsSelected()
        {
            const int numberOfPlayers = 10;

            var teams = new List<SelectedTeam>();
            var players = CreatePlayer(numberOfPlayers);
            for (int i = 0; i < numberOfPlayers; i++)
            {
                teams.Add(SelectedTeam.Create(players[i], TournamentTeamNames.Teams[i]));
            }

            var result = new TournamentDomainService(GetPersistanceClient()).DrawTeamForPlayers(teams);
            result.Count.Should().Be(result.Count);
        }

        [Test]
        public void AnyTeamOmitted()
        {
            const int numberOfPlayers = 10;

            var teams = new List<SelectedTeam>();
            var players = CreatePlayer(numberOfPlayers);
            for (int i = 0; i < numberOfPlayers; i++)
            {
                teams.Add(SelectedTeam.Create(players[i], TournamentTeamNames.Teams[i]));
            }

            var result = new TournamentDomainService(GetPersistanceClient()).DrawTeamForPlayers(teams);
            var resultTeamNames = result.Select(r => r.TeamName);
            teams.Select(t => t.TeamName).All(t => resultTeamNames.Any(tn => tn == t))
                .Should().BeTrue();
        }


        [Test]
        public void AnyPlayerOmitted()
        {
            const int numberOfPlayers = 10;

            var teams = new List<SelectedTeam>();
            var players = CreatePlayer(numberOfPlayers);
            for (int i = 0; i < numberOfPlayers; i++)
            {
                teams.Add(SelectedTeam.Create(players[i], TournamentTeamNames.Teams[i]));
            }

            var result = new TournamentDomainService(GetPersistanceClient()).DrawTeamForPlayers(teams);
            var resultPlayers = result.Select(r => r.Player);
            teams.Select(p => p.Player).All(p => resultPlayers.Any(tn => tn == p))
                .Should().BeTrue();
        }

        private List<Player> CreatePlayer(int numberOfPlayers)
        {
            var result = new List<Player>();
            for(int i = 0; i < numberOfPlayers; i++)
            {
                result.Add(Player.Create(i.ToString(), i.ToString()));
            }
            return result;
        }

        private IPersistanceClient GetPersistanceClient()
            => _scope.ServiceProvider.GetRequiredService<IPersistanceClient>();
    }
}
