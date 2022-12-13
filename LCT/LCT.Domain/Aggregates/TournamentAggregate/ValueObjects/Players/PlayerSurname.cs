using LCT.Domain.Common.Aggregates.TournamentAggregate.ValueObjects;

namespace LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players
{
    public class PlayerSurname : Name
    {
        PlayerSurname() { }
        private PlayerSurname(string name) : base(name)
        {
        }

        public static PlayerSurname Create(string name)
            => new PlayerSurname(name);
    }
}
