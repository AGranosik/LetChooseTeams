namespace LCT.Core.Shared
{
    public class Entity<TKey>
    {
        protected Entity(TKey id)
        {
            Id = id;
        }
        public TKey Id { get; private set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
    }
}
