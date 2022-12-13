using LCT.Domain.Common.Aggregates.TournamentAggregate.ValueObjects;

namespace LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players
{
    public class PlayerName : Name
    {
        PlayerName() { }
        private PlayerName(string name) : base(name)
        {
        }

        public static PlayerName Create(string name)
            => new PlayerName(name);
    }
}
