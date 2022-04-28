using FluentAssertions;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.Services;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Core.Entities.Tournaments.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LCT.IntegrationTests.Tournaments.DomainTests.Services
{
    [TestFixture]
    public class DrawnTeamServiceTests
    {
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

            var result = new TournamentDomainService().DrawTeamForPlayers(teams);
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

            var result = new TournamentDomainService().DrawTeamForPlayers(teams);
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

            var result = new TournamentDomainService().DrawTeamForPlayers(teams);
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

            var result = new TournamentDomainService().DrawTeamForPlayers(teams);
            var resultPlayers = result.Select(r => r.Player);
            teams.Select(p => p.Player).All(p => resultPlayers.Any(tn => tn == p))
                .Should().BeTrue();
        }

        [Test]
        public void TeamsShuffledProperly()
        {
            const int numberOfPlayers = 10;

            var teams = new List<SelectedTeam>();
            var players = CreatePlayer(numberOfPlayers);
            for (int i = 0; i < numberOfPlayers; i++)
            {
                teams.Add(SelectedTeam.Create(players[i], TournamentTeamNames.Teams[i]));
            }

            var result = new TournamentDomainService().DrawTeamForPlayers(teams);
            result.Any(r => teams.Any(t => t.Player == r.Player && t.TeamName == r.TeamName))
                .Should().BeFalse();
        }


        private List<Player> CreatePlayer(int numberOfPlayers)
        {
            var result = new List<Player>();
            for(int i = 0; i < numberOfPlayers; i++)
            {
                result.Add(Player.Register(new Name(i.ToString()), new Name(i.ToString())));
            }
            return result;
        }
    }
}
