using LCT.Domain.Common.BaseTypes;
using LCT.Domain.Common.Interfaces;

namespace LCT.Domain.Aggregates.TournamentAggregate.Events
{
    public class SetTournamentNameEvent : DomainEvent, IVersionable, IUniqness
    {
        public string TournamentName { get; set; }
        public string UniqueValue { get => TournamentName; }

        public SetTournamentNameEvent(string tournamentName, Guid streamId) : base(streamId)
        {
            TournamentName = tournamentName;
        }
    }
}
