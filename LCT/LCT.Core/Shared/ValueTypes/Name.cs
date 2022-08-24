using LCT.Core.Shared.BaseTypes;
using static LCT.Core.Shared.Validation.FieldValidationExtension;

namespace LCT.Domain.Aggregates.TournamentAggregate.ValueObjects
{
    public abstract class Name: ValueType<Name>
    {
        public Name(string name)
        {
            Validate(name);
            Value = name;
        }

        public string Value { get; init; }

        protected virtual void Validate(string name)
        {
            CheckIfNullOrEmpty(name, nameof(Name));
            CheckFieldLength(name, nameof(Name));
        }

        public static implicit operator string(Name name) => name?.Value;

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
