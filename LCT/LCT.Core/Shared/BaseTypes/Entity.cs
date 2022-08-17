namespace LCT.Core.Shared.BaseTypes
{
    public abstract class Entity<TKey>
        where TKey : class
    {
        protected Entity(TKey id)
        {
            Id = id;
        }
        public TKey Id { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime LastModifiedAt { get; private set; }

        public override bool Equals(object obj)
        {
            var other = obj as Entity<TKey>;
            if (other is null)
                return false;

            return Id == other.Id;
        }

        public static bool operator ==(Entity<TKey> a, Entity<TKey> b)
            => a.Equals(b);

        public static bool operator !=(Entity<TKey> a, Entity<TKey> b) => !(a == b);
    }
}
