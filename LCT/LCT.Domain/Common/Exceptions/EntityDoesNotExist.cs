namespace LCT.Domain.Common.Exceptions
{
    public class EntityDoesNotExist : Exception
    {
        public EntityDoesNotExist(string name) : base(name)
        {

        }
    }
}
