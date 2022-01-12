using static LCT.Core.Shared.Validation.FieldValidationExtension;

namespace LCT.Core.Entites.Tournament.ValueObjects
{
    public class Name
    {
        public Name(string name)
        {
            Validate(name);
            Value = name;
        }
        public string Value { get; private set; }

        public static void Validate(string name)
        {
            CheckIfNullOrEmpty(name, nameof(Name));
            CheckFieldLength(name, nameof(Name));
        }

        public static implicit operator string(Name name) => name.Value;

        public static implicit operator Name(string name) => new Name(name);

        public override string ToString() => Value;

    }


}
