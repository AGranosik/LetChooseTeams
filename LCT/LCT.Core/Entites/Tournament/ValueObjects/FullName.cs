using static LCT.Core.Shared.Validation.FieldValidationExtension;

namespace LCT.Core.Entites.Tournament.ValueObjects
{
    public class FullName : IEquatable<FullName>
    {
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public FullName(string name, string surname)
        {
            Validate(name, surname);
            Name = name;
            Surname = surname;
        }
        public bool Equals(FullName other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && Surname == other.Surname;
        }
        
        public static void Validate(string name, string surname)
        {
            CheckIfNullOrEmpty(name, nameof(Name));
            CheckIfNullOrEmpty(surname, nameof(Surname));
            CheckFieldLength(name, nameof(Name), 3, 80);
            CheckFieldLength(surname, nameof(Surname), 3, 80);
        }
    }
}
