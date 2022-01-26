using FluentAssertions;
using LCT.Core.Entites.Tournament.Entities;
using LCT.Core.Entites.Tournament.ValueObjects;
using LCT.Core.Shared.Exceptions;
using NUnit.Framework;

namespace LCT.IntegrationTests.Tournaments.DomainTests
{
    public class TournamentLimitValidation
    {
        [Test]
        public void TournamentLimit_CannotBeNegative()
        {
            var func = () => Tournament.Create(new Name("test"), new TournamentLimit(-1));
            func.Should().Throw<ValueSmallerThanMinimalValueException>();
        }
    }
}
