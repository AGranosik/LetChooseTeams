using LCT.Core.Shared;
using static LCT.Core.Shared.Validation.FieldValidationExtension;

namespace LCT.Core.Aggregates.TournamentAggregate.ValueObjects
{
    public abstract class Name: ValueType<Name>
    {
        public Name(string name)
        {
            Validate(name);
            Value = name;
        }
        public string Value { get; private set; }

        protected virtual void Validate(string name)
        {
            CheckIfNullOrEmpty(name, nameof(TournamentName));
            CheckFieldLength(name, nameof(TournamentName));
        }

        public override string ToString() => Value;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var name = obj as Name;

            if (name == null)
                return false;

            return Value == name.Value;
        }
    }
}
