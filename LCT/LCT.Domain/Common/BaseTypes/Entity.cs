using Newtonsoft.Json;

namespace LCT.Domain.Common.BaseTypes
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Entity<TKey> 
        where TKey : ValueType<TKey>
    {
        protected Entity(TKey id)
        {
            Id = id;
        }
        [JsonProperty]
        public TKey Id { get; protected set; }

        public override bool Equals(object obj)
        {
            var other = obj as Entity<TKey>;
            if (other is null)
                return false;

            return Id == other.Id;
        }

        public static bool operator ==(Entity<TKey> a, Entity<TKey> b)
        {
            if(ReferenceEquals(a, b))
                return true;
            return a.Equals(b);
        }

        public static bool operator !=(Entity<TKey> a, Entity<TKey> b) => !(a == b);
    }
}
