using FluentAssertions;
using LCT.Core.Shared.Exceptions;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams;
using NUnit.Framework;

namespace LCT.IntegrationTests.Domain.Core
{
    [TestFixture]
    public class TournamentNameDomainTests
    {
        [Test]
        public void TournamentName_CannotBeNull_ThrowsException()
        {
            var func = () => new TournamentName(null);
            func.Should().Throw<FieldCannotBeEmptyException>();
        }

        [Test]
        public void TournamentName_CannotBeEmpty_ThrowsException()
        {
            var func = () => new TournamentName(string.Empty);
            func.Should().Throw<InvalidFieldException>();
        }

        [Test]
        public void TournamentName_CreationSuccess()
        {
            var func = () => new TournamentName("hehe");
            func.Should().NotThrow<InvalidFieldException>();
        }

        [Test]
        public void TournamentName_BothAreNull_True()
        {
            TournamentName name = null;
            (name == null).Should().BeTrue();
            (name != null).Should().BeFalse();
        }

        [Test]
        public void TournamentName_OneIsNull_False()
        {
            TournamentName name = new TournamentName("hehe");
            TournamentName nullName = null;
            (name == nullName).Should().BeFalse();
            (name != nullName).Should().BeTrue();
            (nullName == name).Should().BeFalse();
            (nullName != name).Should().BeTrue();
        }

        [Test]
        public void TournamentName_DiffValues_NotEqual()
        {
            var name = new TournamentName("hehe");
            var name2 = new TournamentName("fiu fiu");

            (name == name2).Should().BeFalse();
            (name != name2).Should().BeTrue();
        }

        [Test]
        public void TournamentName_SameValues_Equal()
        {
            var name = new TournamentName("hehe");
            var name2 = new TournamentName("hehe");

            (name == name2).Should().BeTrue();
            (name != name2).Should().BeFalse();
        }
    }
}
