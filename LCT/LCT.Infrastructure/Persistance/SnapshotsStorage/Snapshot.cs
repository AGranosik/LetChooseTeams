using System.Text.Json;
using System.Text.Json.Nodes;
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
            SerializedAggregate = JsonSerializer.Serialize(aggregate);
            StreamId = streamId;
        }

        [BsonId]
        public Guid StreamId { get; set; }
        public int EventNumber { get; init; }
        public DateTime CreationDate { get; } = DateTime.UtcNow;

        public string SerializedAggregate { get; init; }
        public TAgregateRoot Aggregate
            => JsonSerializer.Deserialize<TAgregateRoot>(SerializedAggregate);

    }

    public class EventSnapshot<TEvent>
        where TEvent : DomainEvent
    {

    }
}
