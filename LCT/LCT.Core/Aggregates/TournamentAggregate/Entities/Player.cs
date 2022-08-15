using LCT.Core.Aggregates.TournamentAggregate.ValueObjects;
using LCT.Core.Shared;

namespace LCT.Core.Aggregates.TournamentAggregate.Entities
{
    public class Player : Entity
    {
        private Player(): base() { }
        private Player(TournamentName name, TournamentName surname, Guid id): base(){
            Name = name;
            Surname = surname;
            Id = id;
        }
        public TournamentName Name { get; private set; }
        public TournamentName Surname { get; private set; }

        public static Player Register(TournamentName name, TournamentName surname, Guid id)
            => new (name, surname, id);

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
