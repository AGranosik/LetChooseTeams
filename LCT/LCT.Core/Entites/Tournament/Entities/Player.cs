using LCT.Core.Entites.Tournament.ValueObjects;

namespace LCT.Core.Entites.Tournament.Entities
{
    public class Player : Entity
    {
        private Player(): base() { }
        public Player(Guid id, Name name, Name surname): base(id){
            Name = name;
            Surname = surname;
        }
        public Name Name { get; private set; }
        public Name Surname { get; private set; }
    }
}
