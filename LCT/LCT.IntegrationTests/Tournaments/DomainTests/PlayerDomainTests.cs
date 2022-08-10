using FluentAssertions;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.ValueObjects;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
