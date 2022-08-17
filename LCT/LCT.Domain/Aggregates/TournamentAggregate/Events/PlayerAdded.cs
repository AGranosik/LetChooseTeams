using LCT.Core.Shared.BaseTypes;
using MongoDB.Bson.Serialization.Attributes;

namespace LCT.Domain.Aggregates.TournamentAggregate.Events
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator("PlayerAdded")]
    public class PlayerAdded : BaseEsEvent
    {
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public Guid PlayerId { get; set; }
        public PlayerAdded(string name, string surname, Guid playerId, Guid streamId) : base(streamId)
        {
            Name = name;
            Surname = surname;
            PlayerId = playerId;
        }
    }
}
