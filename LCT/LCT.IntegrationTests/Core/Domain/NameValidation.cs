using FluentAssertions;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Core.Shared.Exceptions;
using NUnit.Framework;

namespace LCT.IntegrationTests.Domain.Core
{
    [TestFixture]
    public class NameValidation
    {
        [Test]
        public void Name_CannotBeNull()
        {
            var func = () => new TournamentName(null);
            func.Should().Throw<FieldCannotBeEmptyException>();
        }

        [Test]
        public void Name_CannotBeEmpty()
        {
            var func = () => new TournamentName(string.Empty);
            func.Should().Throw<InvalidFieldException>();
        }
    }
}
