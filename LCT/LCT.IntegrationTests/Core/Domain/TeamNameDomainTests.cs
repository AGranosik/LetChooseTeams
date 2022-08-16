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
        public void TeamName_CannotBeEmpty_ThrowsException()
        {
            var func = () => new TeamName(string.Empty);
            func.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void TeamName_NotFromAvailableTeam_ThrowsException()
        {
            var func = () => new TeamName("sss");
            func.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void TeamName_SelectedFromAvailableTeams_Success()
        {
            var func = () => new TeamName(TournamentTeamNames.Teams.First());
            func.Should().NotThrow<ArgumentNullException>();
        }

        [Test]
        public void TeamName_SameEntityComparision_True()
        {
            var name = new TeamName(TournamentTeamNames.Teams.First());
            (name == name).Should().BeTrue();
        }

        [Test]
        public void TeamName_BothAreNull_True()
        {
            TeamName name1 = null;
            TeamName name2 = null;

            (name1 == name2).Should().BeTrue();
            (name1 != name2).Should().BeFalse();
        }

        [Test]
        public void TeamName_OneIsNull_False()
        {
            TeamName name1 = new TeamName(TournamentTeamNames.Teams.First());
            TeamName name2 = null;

            (name1 == name2).Should().BeFalse();
            (name1 != name2).Should().BeTrue();

            (name2 == name1).Should().BeFalse();
            (name2 != name1).Should().BeTrue();
        }

        [Test]
        public void TeamName_SameNameComparision_True()
        {
            var name = new TeamName(TournamentTeamNames.Teams.First());
            var name2 = new TeamName(TournamentTeamNames.Teams.First());
            (name == name2).Should().BeTrue();
        }

        [Test]
        public void TeamName_DiffrentNameComparision_False()
        {
            var name = new TeamName(TournamentTeamNames.Teams.First());
            var name2 = new TeamName(TournamentTeamNames.Teams.Last());
            (name == name2).Should().BeFalse();
        }
    }
}
