using LCT.Core.Shared.BaseTypes;
using MongoDB.Bson.Serialization.Attributes;

namespace LCT.Domain.Aggregates.TournamentAggregate.Events
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator("TournamentCreated")]
    public class TournamentCreated : DomainEvent
    {
        public string Name { get; set; }
        public int Limit { get; set; }
        public TournamentCreated(string tournamentName, int limit, Guid streamId): base(streamId)
        {
            Name = tournamentName;
            Limit = limit;
        }
    }
}
