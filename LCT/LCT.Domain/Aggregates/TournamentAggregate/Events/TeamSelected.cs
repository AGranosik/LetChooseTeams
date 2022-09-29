using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players;
using LCT.Domain.Common.BaseTypes;
using MongoDB.Bson.Serialization.Attributes;

namespace LCT.Domain.Aggregates.TournamentAggregate.Events
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator("TeamSelected")]
    public class TeamSelected : DomainEvent
    {
        public string TeamName { get; set; }
        public Player Player { get; set; }

        public TeamSelected(Player player, string teamName, Guid streamId) : base(streamId)
        {
            TeamName = teamName;
            Player = player;
        }
    }
}
