
using LCT.Core.Entites.Tournaments.ValueObjects;

namespace LCT.Core.Entites.Tournaments.Entities
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
                return a.Name == b.Name
                    && b.Surname == a.Surname;
            }

            return false;
        }

        public static bool operator !=(Player a, Player b) => !(a == b);
    }
}
