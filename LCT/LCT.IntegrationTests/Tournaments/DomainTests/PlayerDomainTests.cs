using FluentAssertions;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects;
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
            var player1 = Player.Create("test", "test");
            var player2 = Player.Create("test", "test2");

            (player1 == player2).Should().BeFalse();
        }

        [Test]
        public void EqualityTest_DifferentNameSurname_ReturnFalse()
        {
            var player1 = Player.Create("test", "test");
            var player2 = Player.Create("test", "test2");

            (player1 == player2).Should().BeFalse();
        }

        [Test]
        public void EqualityTest_ReturnTrue()
        {
            var player1 = Player.Create("test", "test");
            var player2 = Player.Create("test", "test");

            (player1 == player2).Should().BeTrue();
        }

    }
}
