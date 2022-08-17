using FluentAssertions;
using LCT.Core.Shared.Exceptions;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects;
using NUnit.Framework;

namespace LCT.IntegrationTests.Core.Domain
{
    [TestFixture]
    public class PlayerNameDomainTests
    {
        [Test]
        public void PlayerName_CannotBeNull_ThrowsException()
        {
            var func = () => new PlayerName(null, null);
            func.Should().Throw<FieldCannotBeEmptyException>();
        }

        [Test]
        public void PlayerName_NameIsNull_ThrowsException()
        {
            var func = () => new PlayerName(null, "hehe");
            func.Should().Throw<FieldCannotBeEmptyException>();
        }

        [Test]
        public void PlayerName_SurnameIsNull_ThrowsException()
        {
            var func = () => new PlayerName("hehe", null);
            func.Should().Throw<FieldCannotBeEmptyException>();
        }

        [Test]
        public void PlayerName_CannotBeEmpty_ThrowsException()
        {
            var func = () => new PlayerName(string.Empty, "hehe");
            func.Should().Throw<InvalidFieldException>();

            var func2 = () => new PlayerName("hehe", string.Empty);
            func2.Should().Throw<InvalidFieldException>();
        }

        [Test]
        public void PlayerName_CreationSuccess()
        {
            var func = () => new PlayerName("hehe", "fiu fiu");
            func.Should().NotThrow<InvalidFieldException>();
        }

        [Test]
        public void PlayerNameCompare_BothAreNull_True()
        {
            PlayerName name = null;
            (name == null).Should().BeTrue();
            (name != null).Should().BeFalse();
        }

        [Test]
        public void PlayerName_OneIsNull_False()
        {
            PlayerName name = new PlayerName("hehe", "fiu fiu");
            PlayerName nullName = null;
            (name == nullName).Should().BeFalse();
            (name != nullName).Should().BeTrue();
            (nullName == name).Should().BeFalse();
            (nullName != name).Should().BeTrue();
        }

        [Test]
        public void PlayerName_DiffValues_NotEqual()
        {
            var name = new PlayerName("hehe", "fiu fiu");
            var name2 = new PlayerName("hehe2", "fiu fiu");

            (name == name2).Should().BeFalse();
            (name != name2).Should().BeTrue();
        }

        [Test]
        public void PlayerName_SameValues_Equal()
        {
            var name = new PlayerName("hehe", "fiu fiu");
            var name2 = new PlayerName("hehe", "fiu fiu");

            (name == name2).Should().BeTrue();
            (name != name2).Should().BeFalse();
        }
    }
}
