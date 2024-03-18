namespace LCT.Infrastructure.Common.Exceptions
{
    public class EntityDoesNotExist : Exception
    {
        public EntityDoesNotExist(string entityName): base(entityName)
        {
            
        }
    }
}
