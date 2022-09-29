using LCT.Domain.Common.Aggregates.TournamentAggregate.ValueObjects;

namespace LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players
{
    public class PlayerSurname : Name
    {
        public PlayerSurname(string name) : base(name)
        {
        }
    }
}
