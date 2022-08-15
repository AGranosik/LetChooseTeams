using Microsoft.EntityFrameworkCore;
using static LCT.Core.Shared.Validation.FieldValidationExtension;

namespace LCT.Core.Aggregates.TournamentAggregate.ValueObjects
{
    [Keyless]
    public class TournamentName
    {
        public TournamentName()
        {

        }
        public TournamentName(string name)
        {
            Validate(name);
            Value = name;
        }
        public string Value { get; private set; }

        public static void Validate(string name)
        { 
            CheckIfNullOrEmpty(name, nameof(TournamentName));
            CheckFieldLength(name, nameof(TournamentName));
        }

        public static implicit operator string(TournamentName name) => name.Value;

        public static implicit operator TournamentName(string name) => new (name);

        public static bool operator ==(TournamentName a, TournamentName b)
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

        public static bool operator !=(TournamentName a, TournamentName b) => !(a == b);

        public override string ToString() => Value;

    }


}
