namespace LCT.Core.Entites
{
    public class Entity
    {
        protected Entity()
        {

        }
        public Entity(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; private set; }
        public DateTime CreatedAt { get; private set; }
    }
}
