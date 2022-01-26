using FluentAssertions;
using LCT.Core.Entites.Tournament.ValueObjects;
using LCT.Core.Shared.Exceptions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var func = () => new Name(String.Empty);
            func.Should().Throw<InvalidFieldException>();
        }
    }
}
