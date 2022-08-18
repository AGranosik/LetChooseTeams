using FluentAssertions;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams;
using NUnit.Framework;
using System;
using System.Linq;

namespace LCT.IntegrationTests.Teams.DomainTests
{
    [TestFixture]
    public class SelectTeamDomainTests
    {
        private readonly Player _player = Player.Create("sdasd", "hehe");

        [Test]
        public void Player_Null_ThrowsException()
        {
            var func = () => SelectedTeam.Create(null, "hehe");
            func.Should().Throw<ArgumentNullException>();
        }

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
