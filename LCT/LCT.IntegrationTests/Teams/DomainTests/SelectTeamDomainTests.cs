using FluentAssertions;
using LCT.Core.Aggregates.TournamentAggregate.Entities;
using LCT.Core.Aggregates.TournamentAggregate.Types;
using NUnit.Framework;
using System;
using System.Linq;

namespace LCT.IntegrationTests.Teams.DomainTests
{
    [TestFixture]
    public class SelectTeamDomainTests
    {
        private readonly Player _player = Player.Register("sdasd", "hehe", Guid.NewGuid());

        [Test]
        public void TeamNameCannotBeEmpty()
        {
            var func = () => SelectedTeam.Create(_player, string.Empty);
            func.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void TeamNameNotCorrect()
        {
            var func = () => SelectedTeam.Create(_player, "hehe");
            func.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void TeamNameSuccess()
        {
            var func = () => SelectedTeam.Create(_player, TournamentTeamNames.Teams.First());
            func.Should().NotThrow();
        }
    }
}
