namespace LCT.Core.Shared.BaseTypes
{
    public abstract class Entity<TKey> 
        where TKey : ValueType<TKey>
    {
        protected Entity(TKey id)
        {
            Id = id;
        }
        public TKey Id { get; private set; }

        public override bool Equals(object obj)
        {
            var other = obj as Entity<TKey>;
            if (other is null)
                return false;

            return Id == other.Id;
        }

        public static bool operator ==(Entity<TKey> a, Entity<TKey> b)
            => a.Id.Equals(b.Id);

        public static bool operator !=(Entity<TKey> a, Entity<TKey> b) => !(a == b);
    }
}
