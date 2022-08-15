using FluentAssertions;
using LCT.Core.Aggregates.TournamentAggregate.Types;
using LCT.Core.Aggregates.TournamentAggregate.ValueObjects;
using NUnit.Framework;
using System;
using System.Linq;

namespace LCT.IntegrationTests.Core.Domain
{
    [TestFixture]
    public class TeamNameDomainTests
    {
        [Test]
        public void CannotBeEmpty()
        {
            var func = () => new TeamName(string.Empty);
            func.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void NotFromAvailableTeam()
        {
            var func = () => new TeamName("sss");
            func.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void SelectedFromAvailableTeams()
        {
            var func = () => new TeamName(TournamentTeamNames.Teams.First());
            func.Should().NotThrow<ArgumentNullException>();
        }

        [Test]
        public void SameEntityComparision_True()
        {
            var name = new TeamName(TournamentTeamNames.Teams.First());
            (name == name).Should().BeTrue();
        }

        [Test]
        public void SameNameComparision_True()
        {
            var name = new TeamName(TournamentTeamNames.Teams.First());
            var name2 = new TeamName(TournamentTeamNames.Teams.First());
            (name == name2).Should().BeTrue();
        }

        [Test]
        public void DiffrentNameComparision_False()
        {
            var name = new TeamName(TournamentTeamNames.Teams.First());
            var name2 = new TeamName(TournamentTeamNames.Teams.Last());
            (name == name2).Should().BeFalse();
        }
    }
}
