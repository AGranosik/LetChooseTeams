using FluentAssertions;
using LCT.Core.Entites.Tournament.ValueObjects;
using LCT.Core.Shared.Exceptions;
using NUnit.Framework;
using System;

namespace LCT.IntegrationTests.Core
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
