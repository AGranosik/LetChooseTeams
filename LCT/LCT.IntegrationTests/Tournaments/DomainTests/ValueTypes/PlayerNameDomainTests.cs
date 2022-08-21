using FluentAssertions;
using LCT.Core.Shared.Exceptions;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players;
using NUnit.Framework;

namespace LCT.IntegrationTests.Tournaments.DomainTests.ValueTypes
{
    [TestFixture]
    public class PlayerNameDomainTests
    {
        [Test]
        public void PlayerName_CannotBeNull_ThrowsException()
        {
            var func = () => Player.Create(null, null);
            func.Should().Throw<FieldCannotBeEmptyException>();
        }

        [Test]
        public void PlayerName_NameIsNull_ThrowsException()
        {
            var func = () => Player.Create(null, "hehe");
            func.Should().Throw<FieldCannotBeEmptyException>();
        }

        [Test]
        public void PlayerName_SurnameIsNull_ThrowsException()
        {
            var func = () => Player.Create("hehe", null);
            func.Should().Throw<FieldCannotBeEmptyException>();
        }

        [Test]
        public void PlayerName_CannotBeEmpty_ThrowsException()
        {
            var func = () => Player.Create(string.Empty, "hehe");
            func.Should().Throw<InvalidFieldException>();

            var func2 = () => Player.Create("hehe", string.Empty);
            func2.Should().Throw<InvalidFieldException>();
        }

        [Test]
        public void PlayerName_CreationSuccess()
        {
            var func = () => Player.Create("hehe", "fiu fiu");
            func.Should().NotThrow<InvalidFieldException>();
        }

        [Test]
        public void PlayerNameCompare_BothAreNull_True()
        {
            Player name = null;
            (name == null).Should().BeTrue();
            (name != null).Should().BeFalse();
        }

        [Test]
        public void PlayerName_OneIsNull_False()
        {
            Player name = Player.Create("hehe", "fiu fiu");
            Player nullName = null;
            (name == nullName).Should().BeFalse();
            (name != nullName).Should().BeTrue();
            (nullName == name).Should().BeFalse();
            (nullName != name).Should().BeTrue();
        }

        [Test]
        public void PlayerName_DiffValues_NotEqual()
        {
            var name = Player.Create("hehe", "fiu fiu");
            var name2 = Player.Create("hehe2", "fiu fiu");

            (name == name2).Should().BeFalse();
            (name != name2).Should().BeTrue();
        }

        [Test]
        public void PlayerName_SameValues_Equal()
        {
            var name = Player.Create("hehe", "fiu fiu");
            var name2 = Player.Create("hehe", "fiu fiu");

            (name == name2).Should().BeTrue();
            (name != name2).Should().BeFalse();
        }
    }
}
