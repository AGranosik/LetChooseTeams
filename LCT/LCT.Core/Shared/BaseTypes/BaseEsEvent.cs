using MongoDB.Bson.Serialization.Attributes;

namespace LCT.Core.Shared.BaseTypes
{
    [BsonIgnoreExtraElements]
    public abstract class BaseEsEvent
    {
        public BaseEsEvent(Guid streamId)
        {
            StreamId = streamId;
        }
        public Guid Id { get; set; }
        public Guid StreamId { get; private set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
