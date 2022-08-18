using LCT.Core.Shared.BaseTypes;
using static LCT.Core.Shared.Validation.FieldValidationExtension;

namespace LCT.Domain.Aggregates.TournamentAggregate.ValueObjects
{
    public class Player : ValueType<Player>
    {
        public static Player Create(string name, string surname)
            => new(name, surname);
        public string Name { get; private set; }
        public string Surname { get; private set; }
        private Player(string name, string surname)
        {
            Validate(name);
            Validate(surname);
            Name = name;
            Surname = surname;
        }

        private void Validate(string field)
        {
            CheckIfNullOrEmpty(field, nameof(Player));
            CheckFieldLength(field, nameof(Player));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            Player other = obj as Player;
            if(other == null) return false;
            return Name == other.Name && Surname == other.Surname;
        }
    }
}
