using FluentAssertions;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Core.Entities.Tournaments.Types;
using NUnit.Framework;
using System;
using System.Linq;

namespace LCT.IntegrationTests.Teams.DomainTests
{
    [TestFixture]
    public class SelectTeamDomainTests
    {
        private readonly Player _player = Player.Register(new Name("sdasd"), new Name("hehe"));

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
            var func = () => SelectedTeam.Create(_player, TournamentTeamNames._teams.First());
            func.Should().NotThrow();
        }
    }
}
