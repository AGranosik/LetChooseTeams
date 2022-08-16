using LCT.Core.Shared;
using Microsoft.EntityFrameworkCore;
using static LCT.Core.Shared.Validation.FieldValidationExtension;

namespace LCT.Core.Aggregates.TournamentAggregate.ValueObjects
{
    public class TournamentName : ValueType<TournamentName>
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

        public override string ToString() => Value;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var tournamentName = obj as TournamentName;

            if (tournamentName == null)
                return false;

            return Value == tournamentName.Value;
        }
    }


}
