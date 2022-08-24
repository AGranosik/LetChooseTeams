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
        public Guid Id { get; init; }
        public Guid StreamId { get; init; }
        public DateTime TimeStamp { get; init; } = DateTime.UtcNow;
    }
}
