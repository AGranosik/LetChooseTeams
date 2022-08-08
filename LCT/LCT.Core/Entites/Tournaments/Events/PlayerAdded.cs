using MongoDB.Bson.Serialization.Attributes;

namespace LCT.Core.Entites.Tournaments.Events
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator("PlayerAdded")]
    public class PlayerAdded : BaseEvent
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
