namespace LCT.Core.Shared.Exceptions
{
    public class InvalidFieldException : Exception
    {
        public InvalidFieldException(string field): base($@"Invalid field: {field}")
        {

        }
    }
}
