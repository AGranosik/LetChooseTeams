namespace LCT.Domain.Common.Exceptions
{
    public class FieldCannotBeEmptyException : Exception
    {
        public FieldCannotBeEmptyException(string field) : base($@"Field: {field} cannot be empty")
        {

        }
    }
}
