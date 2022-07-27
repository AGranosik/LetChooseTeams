using MongoDB.Bson.Serialization.Attributes;

namespace LCT.Core.Entites.Tournaments.Events
{
    [BsonIgnoreExtraElements]
    public abstract class BaseEvent
    {
        public BaseEvent(Guid id)
        {
            StreamId= id;
        }
        public string EventType => this.GetType().ToString();
        public Guid StreamId { get; private set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
