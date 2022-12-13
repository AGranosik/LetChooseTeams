using FluentAssertions;
using LCT.Core.Shared.Exceptions;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams;
using LCT.Domain.Common.Exceptions;
using NUnit.Framework;

namespace LCT.IntegrationTests.Tournaments.DomainTests.ValueTypes
{
    [TestFixture]
    public class TournamentNameDomainTests
    {
        [Test]
        public void TournamentName_CannotBeNull_ThrowsException()
        {
            var func = () => TournamentName.Create(null);
            func.Should().Throw<FieldCannotBeEmptyException>();
        }

        [Test]
        public void TournamentName_CannotBeEmpty_ThrowsException()
        {
            var func = () => TournamentName.Create(string.Empty);
            func.Should().Throw<InvalidFieldException>();
        }

        [Test]
        public void TournamentName_CreationSuccess()
        {
            var func = () => TournamentName.Create("hehe");
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
            TournamentName name = TournamentName.Create("hehe");
            TournamentName nullName = null;
            (name == nullName).Should().BeFalse();
            (name != nullName).Should().BeTrue();
            (nullName == name).Should().BeFalse();
            (nullName != name).Should().BeTrue();
        }

        [Test]
        public void TournamentName_DiffValues_NotEqual()
        {
            var name = TournamentName.Create("hehe");
            var name2 = TournamentName.Create("fiu fiu");

            (name == name2).Should().BeFalse();
            (name != name2).Should().BeTrue();
        }

        [Test]
        public void TournamentName_SameValues_Equal()
        {
            var name = TournamentName.Create("hehe");
            var name2 = TournamentName.Create("hehe");

            (name == name2).Should().BeTrue();
            (name != name2).Should().BeFalse();
        }
    }
}
