using LCT.Core.Shared;
using static LCT.Core.Shared.Validation.FieldValidationExtension;

namespace LCT.Core.Aggregates.TournamentAggregate.ValueObjects
{
    public class PlayerName : ValueType<PlayerName>
    {
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public PlayerName(string name, string surname)
        {
            Validate(name);
            Validate(surname);
            Name = name;
            Surname = surname;
        }

        private void Validate(string field)
        {
            CheckIfNullOrEmpty(field, nameof(PlayerName));
            CheckFieldLength(field, nameof(PlayerName));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            PlayerName other = obj as PlayerName;
            if(other == null) return false;
            return Name == other.Name && Surname == other.Surname;
        }
    }
}
