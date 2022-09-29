using LCT.Domain.Common.BaseTypes;
using MongoDB.Bson.Serialization.Attributes;

namespace LCT.Domain.Aggregates.TournamentAggregate.Events
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator("PlayerAdded")]
    public class PlayerAdded : DomainEvent
    {
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public PlayerAdded(string name, string surname, Guid streamId) : base(streamId)
        {
            Name = name;
            Surname = surname;
        }
    }
}
