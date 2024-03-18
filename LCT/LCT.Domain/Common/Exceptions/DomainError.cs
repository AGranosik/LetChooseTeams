namespace LCT.Domain.Common.Exceptions
{
    public class DomainError : Exception
    {
        public DomainError()
        {
            
        }
        public DomainError(string message) : base(message)
        {
                
        }
    }
}
