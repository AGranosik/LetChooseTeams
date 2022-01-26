using LCT.Core.Entites.Tournament.ValueObjects;

namespace LCT.Core.Entites.Tournament.Entities
{
    public class Player : Entity
    {
        private Player(): base() { }
        private Player(Name name, Name surname): base(){
            Name = name;
            Surname = surname;
        }
        public Name Name { get; private set; }
        public Name Surname { get; private set; }
        private List<Tournament> _tournamentList = new List<Tournament>();
        public IReadOnlyCollection<Tournament> Tournaments => _tournamentList.AsReadOnly();

        public static Player Register(Name name, Name surname)
            => new (name, surname);

        public static bool operator ==(Player a, Player b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (a is not null && b is not null)
            {
                return a.Id.Equals(b.Id) 
                    && a.Name == b.Name
                    && b.Name == a.Name;
            }

            return false;
        }

        public static bool operator !=(Player a, Player b) => !(a == b);
    }
}
