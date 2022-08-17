namespace LCT.Core.Shared.BaseTypes
{
    public abstract class ValueType<T> where T : ValueType<T>
    {
        public abstract override bool Equals(object obj);
        public static bool operator ==(ValueType<T> left, ValueType<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ValueType<T> left, ValueType<T> right)
        {
            return !(left == right);
        }
    }
}
