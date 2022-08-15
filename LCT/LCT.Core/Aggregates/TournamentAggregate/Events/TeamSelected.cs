using MongoDB.Bson.Serialization.Attributes;

namespace LCT.Core.Aggregates.TournamentAggregate.Events
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator("TeamSelected")]
    public class TeamSelected : BaseEvent
    {
        public string TeamName { get; set; }
        public Guid PlayerId { get; set; }

        public TeamSelected(string teamName, Guid playerId, Guid streamId) : base(streamId)
        {
            TeamName = teamName;
            PlayerId = playerId;
        }
    }
}
