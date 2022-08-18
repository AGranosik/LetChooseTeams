namespace LCT.Core.Shared.BaseTypes
{
    public abstract class BaseId<TKey, TType> : ValueType<TKey>
        where TKey : ValueType<TKey>
    {
        protected BaseId(TType id)
        {
            Value = id;
        }
        public TType Value { get; private set; }
        public override bool Equals(object obj)
        {
            var key = obj as BaseId<TKey, TType>;
            if(key == null)
                return false;

            return Value.Equals(key.Value);
        }
    }
}
