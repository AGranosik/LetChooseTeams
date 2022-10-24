using LCT.Domain.Common.BaseTypes;
using MongoDB.Bson.Serialization.Attributes;

namespace LCT.Domain.Aggregates.TournamentAggregate.Events
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator("TournamentCreated")]
    public class TournamentCreatedDomainEvent : DomainEvent
    {
        public int Limit { get; set; }
        public TournamentCreatedDomainEvent(string tournamentName, int limit, Guid streamId): base(streamId)
        {
            Limit = limit;
        }
    }
}
