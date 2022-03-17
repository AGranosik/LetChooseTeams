namespace LCT.Core.Entites
{
    public class Entity
    {
        protected Entity()
        {
        }
        public Guid Id { get; private set; }
        public DateTime CreatedAt { get; private set; }
    }
}
