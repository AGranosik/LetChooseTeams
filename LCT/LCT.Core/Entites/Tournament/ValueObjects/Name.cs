using Microsoft.EntityFrameworkCore;
using static LCT.Core.Shared.Validation.FieldValidationExtension;

namespace LCT.Core.Entites.Tournament.ValueObjects
{
    [Keyless]
    public class Name
    {
        public Name()
        {

        }
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

        public static implicit operator Name(string name) => new (name);

        public static bool operator ==(Name a, Name b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (a is not null && b is not null)
            {
                return a.Value.Equals(b.Value);
            }

            return false;
        }

        public static bool operator !=(Name a, Name b) => !(a == b);

        public override string ToString() => Value;

    }


}
