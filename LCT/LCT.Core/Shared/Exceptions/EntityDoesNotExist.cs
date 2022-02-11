namespace LCT.Core.Shared.Exceptions
{
    public class EntityDoesNotExist : Exception
    {
        public EntityDoesNotExist(string name) : base(name)
        {

        }
    }
}
