using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCT.Core.Shared.Exceptions
{
    public class FieldCannotBeEmptyException : Exception
    {
        public FieldCannotBeEmptyException(string field) : base($@"Field: {field} cannot be empty")
        {

        }
    }
}
