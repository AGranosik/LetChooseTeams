using MediatR;
using MongoDB.Bson.Serialization.Attributes;

namespace LCT.Domain.Common.BaseTypes
{
    [BsonIgnoreExtraElements]
    public abstract class DomainEvent : INotification
    {
        public DomainEvent(Guid streamId)
        {
            StreamId = streamId;
        }
        public Guid Id { get; init; }
        public Guid StreamId { get; init; }
        public DateTime TimeStamp { get; init; } = DateTime.UtcNow;
    }
}
