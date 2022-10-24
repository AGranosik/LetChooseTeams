using LCT.Domain.Common.BaseTypes;

namespace LCT.Domain.Aggregates.TournamentAggregate.Events
{
    public class SetTournamentNameEvent : DomainEvent
    {
        public string Name { get; set; }
        public SetTournamentNameEvent(string name, Guid streamId) : base(streamId)
        {
            Name = name;
        }
    }
}
