using LCT.Domain.Common.Aggregates.TournamentAggregate.ValueObjects;

namespace LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players
{
    public class PlayerName : Name
    {
        public PlayerName(string name) : base(name)
        {
        }
    }
}
