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
            var player1 = Player.Register(new Name("test"), new Name("test"));
            var player2 = Player.Register(new Name("test"), new Name("test2"));

            (player1 == player2).Should().BeFalse();
        }

        [Test]
        public void EqualityTest_DifferentNameSurname_ReturnFalse()
        {
            var player1 = Player.Register(new Name("test"), new Name("test"));
            var player2 = Player.Register(new Name("test2"), new Name("test2"));

            (player1 == player2).Should().BeFalse();
        }

        [Test]
        public void EqualityTest_ReturnTrue()
        {
            var player1 = Player.Register(new Name("test"), new Name("test"));
            var player2 = Player.Register(new Name("test"), new Name("test"));

            (player1 == player2).Should().BeTrue();
        }

    }
}
