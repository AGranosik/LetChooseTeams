using FluentAssertions;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Core.Shared.Exceptions;
using NUnit.Framework;

namespace LCT.IntegrationTests.Domain.Core
{
    public class NameValidation
    {
        [Test]
        public void Name_CannotBeNull()
        {
            var func = () => new Name(null);
            func.Should().Throw<FieldCannotBeEmptyException>();
        }

        [Test]
        public void Name_CannotBeEmpty()
        {
            var func = () => new Name(string.Empty);
            func.Should().Throw<InvalidFieldException>();
        }
    }
}
