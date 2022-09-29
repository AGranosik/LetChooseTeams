using LCT.Domain.Common.BaseTypes;
using LCT.Domain.Common.Validation;

namespace LCT.Domain.Common.Aggregates.TournamentAggregate.ValueObjects
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
            FieldValidationExtension.CheckIfNullOrEmpty(name, nameof(Name));
            FieldValidationExtension.CheckFieldLength(name, nameof(Name));
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
