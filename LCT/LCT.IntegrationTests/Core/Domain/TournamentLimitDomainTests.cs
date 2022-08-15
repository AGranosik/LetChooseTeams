using FluentAssertions;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Core.Shared.Exceptions;
using NUnit.Framework;

namespace LCT.IntegrationTests.Core.Domain
{
    [TestFixture]
    public class TournamentLimitDomainTests
    {
        [Test]
        public void Limit_CannotBeNegative_ThrowsException()
        {
            var action = () => new TournamentLimit(-2);
            action.Should().Throw<ValueSmallerThanMinimalValueException>();
        }

        [Test]
        public void Limit_HaveToBeGreaterThan1_ThrowsException()
        {
            var action = () => new TournamentLimit(1);
            action.Should().Throw<ValueSmallerThanMinimalValueException>();
        }

        [Test]
        public void Limit_GreaterThan1_Success()
        {
            var action = () => new TournamentLimit(2);
            action.Should().NotThrow<ValueSmallerThanMinimalValueException>();
        }

        [Test]
        public void Limit_DiffValues_NotEqual()
        {
            var lim1 = new TournamentLimit(2);
            var lim2 = new TournamentLimit(4);

            (lim1 == lim2).Should().BeFalse();
            (lim1 != lim2).Should().BeTrue();
        }

        [Test]
        public void Limit_SameValues_Equal()
        {
            var lim1 = new TournamentLimit(4);
            var lim2 = new TournamentLimit(4);

            (lim1 == lim2).Should().BeTrue();
            (lim1 != lim2).Should().BeFalse();
        }
    }
}
