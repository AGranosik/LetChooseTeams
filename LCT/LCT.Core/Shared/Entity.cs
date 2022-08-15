namespace LCT.Core.Shared
{
    public class Entity
    {
        protected Entity()
        {
        }
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
