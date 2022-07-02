using LCT.Core.Entites.Tournaments.ValueObjects;

namespace LCT.Core.Entites.Tournaments.Events
{
    public class TournamentCreated : BaseEvent
    {
        public string Name { get; private set; }
        public int Limit { get; private set; }
        public TournamentCreated(string TournamentName, int Limit)
        {
            this.Name = TournamentName;
            this.Limit = Limit;
        }
    }
}
