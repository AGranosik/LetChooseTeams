using FluentAssertions;
using LCT.Core.Aggregates.TournamentAggregate.Entities;
using NUnit.Framework;
using System;

namespace LCT.IntegrationTests.Tournaments.DomainTests
{
    [TestFixture]
    public class PlayerDomainTests
    {
        [Test]
        public void EqualityTest_DifferentSurname_ReturnFalse()
        {
            var player1 = Player.Register("test", "test", Guid.NewGuid());
            var player2 = Player.Register("test", "test2", Guid.NewGuid());

            (player1 == player2).Should().BeFalse();
        }

        [Test]
        public void EqualityTest_DifferentNameSurname_ReturnFalse()
        {
            var player1 = Player.Register("test", "test", Guid.NewGuid());
            var player2 = Player.Register("test", "test2", Guid.NewGuid());

            (player1 == player2).Should().BeFalse();
        }

        [Test]
        public void EqualityTest_ReturnTrue()
        {
            var player1 = Player.Register("test", "test", Guid.NewGuid());
            var player2 = Player.Register("test", "test", Guid.NewGuid());

            (player1 == player2).Should().BeTrue();
        }

    }
}
