using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players;
using LCT.Domain.Common.BaseTypes;
using MongoDB.Bson.Serialization.Attributes;

namespace LCT.Domain.Aggregates.TournamentAggregate.Events
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator("DrawTeam")]
    public class DrawTeamEvent : DomainEvent
    {
        public string TeamName { get; set; }
        public Player Player { get; set; }

        public DrawTeamEvent(Player player, string teamName, Guid streamId) : base(streamId)
        {
            TeamName = teamName;
            Player = player;
        }
    }
}
