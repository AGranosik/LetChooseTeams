using FluentAssertions;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects;
using NUnit.Framework;
using System;
using System.Linq;

namespace LCT.IntegrationTests.Teams.DomainTests
{
    public class DrawnTeamDomainTests
    {
        private readonly Player _player = Player.Register("sdasd", "hehe", Guid.NewGuid());

        [Test]
        public void TeamNameCannotBeEmpty()
        {
            var func = () => DrawnTeam.Create(_player, string.Empty);
            func.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void TeamNameNotCorrect()
        {
            var func = () => DrawnTeam.Create(_player, "hehe");
            func.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void TeamNameSuccess()
        {
            var func = () => DrawnTeam.Create(_player, TournamentTeamNames.Teams.First());
            func.Should().NotThrow();
        }
    }
}
