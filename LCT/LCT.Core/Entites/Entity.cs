namespace LCT.Core.Entites
{
    public class Entity
    {
        protected Entity()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; private set; }
        public DateTime CreatedAt { get; private set; }
    }
}
