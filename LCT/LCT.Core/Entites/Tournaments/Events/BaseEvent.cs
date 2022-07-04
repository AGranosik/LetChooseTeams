using MongoDB.Bson.Serialization.Attributes;

namespace LCT.Core.Entites.Tournaments.Events
{
    [BsonIgnoreExtraElements]
    public abstract class BaseEvent
    {
        public BaseEvent(Guid id)
        {
            EventId= id;
        }
        public string EventType => this.GetType().ToString();
        public Guid EventId { get; private set; }
        public DateTime TimeStamp = DateTime.UtcNow;
    }
}
