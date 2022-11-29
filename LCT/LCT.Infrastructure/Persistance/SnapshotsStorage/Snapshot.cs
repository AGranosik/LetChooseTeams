using LCT.Domain.Common.BaseTypes;
using MongoDB.Bson.Serialization.Attributes;

namespace LCT.Infrastructure.Persistance.SnapshotsStorage
{
    public class AggregateSnapshot<TAgregateRoot>
        where TAgregateRoot : IAgregateRoot, new()
    {

        public AggregateSnapshot(int version, TAgregateRoot aggregate, Guid streamId)
        {
            EventNumber = version;
            Aggregate = aggregate;
            StreamId = streamId;
        }

        [BsonId]
        public Guid StreamId { get; set; }
        public int EventNumber { get; init; }
        public DateTime CreationDate { get; } = DateTime.UtcNow;

        public TAgregateRoot Aggregate { get; init; }
    }

    public class EventSnapshot<TEvent>
        where TEvent : DomainEvent
    {

    }
}
