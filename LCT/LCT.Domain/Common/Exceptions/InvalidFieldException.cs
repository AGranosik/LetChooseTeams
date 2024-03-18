using LCT.Domain.Common.Exceptions;

namespace LCT.Core.Shared.Exceptions
{
    public class InvalidFieldException : DomainError
    {
        public InvalidFieldException(string field): base($@"Invalid field: {field}")
        {

        }
    }
}
