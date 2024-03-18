namespace LCT.Domain.Common.Exceptions
{
    public class FieldCannotBeEmptyException : DomainError
    {
        public FieldCannotBeEmptyException(string field) : base($@"Field: {field} cannot be empty")
        {

        }
    }
}
