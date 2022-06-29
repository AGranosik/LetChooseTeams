using LCT.Core.Entites.Tournaments.ValueObjects;

namespace LCT.Core.Entites.Tournaments.Events
{
    public class TournamentCreated : BaseEvent
    {
        public Name TournamentName { get; private set; }
        public TournamentLimit Limit { get; private set; }
        public TournamentCreated(Name TournamentName, TournamentLimit Limit)
        {
            this.TournamentName = TournamentName;
            this.Limit = Limit;
        }
    }
}
