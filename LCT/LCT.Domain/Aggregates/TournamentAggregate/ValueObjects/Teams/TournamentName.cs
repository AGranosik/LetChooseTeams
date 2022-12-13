using LCT.Domain.Common.Aggregates.TournamentAggregate.ValueObjects;

namespace LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams
{
    public class TournamentName : Name
    {
        TournamentName(): base() { }
        private TournamentName(string name) : base(name)
        {
        }

        public static TournamentName Create(string name)
            => new (name);
    }


}
