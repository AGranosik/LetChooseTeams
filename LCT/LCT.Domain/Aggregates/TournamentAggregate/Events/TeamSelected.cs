using LCT.Core;
using MongoDB.Bson.Serialization.Attributes;

namespace LCT.Domain.Aggregates.TournamentAggregate.Events
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator("TeamSelected")]
    public class TeamSelected : BaseEsEvent
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
