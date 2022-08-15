using static LCT.Core.Shared.Validation.FieldValidationExtension;

namespace LCT.Core.Entites.Tournaments.ValueObjects
{
    public class PlayerName : IEquatable<PlayerName>
    {
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public PlayerName(string name, string surname)
        {
            Name = name;
            Surname = surname;
        }
        public bool Equals(PlayerName other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && Surname == other.Surname;
        }
    }
}
