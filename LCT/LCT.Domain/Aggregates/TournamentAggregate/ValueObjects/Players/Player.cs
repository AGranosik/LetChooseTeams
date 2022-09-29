using LCT.Domain.Common.BaseTypes;

namespace LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players
{
    public class Player : ValueType<Player>
    {
        public static Player Create(string name, string surname)
            => new(name, surname);
        public PlayerName Name { get; init; }
        public PlayerSurname Surname { get; init; }
        private Player(string name, string surname)
        {
            Name = new PlayerName(name);
            Surname = new PlayerSurname(surname);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            Player other = obj as Player;
            if (other == null) return false;
            return Name == other.Name && Surname == other.Surname;
        }
    }
}
