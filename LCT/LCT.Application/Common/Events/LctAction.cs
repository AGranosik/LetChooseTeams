using MediatR;
using MongoDB.Bson.Serialization.Attributes;

namespace LCT.Application.Common.Events
{
    public abstract class LctAction<AggregateId> : LctAction, INotification
    {
        [BsonId]
        public Guid Id { get; set; }
        public AggregateId GroupKey { get; set; }
        public DateTime SavedTime { get; set; } = DateTime.UtcNow;
    }

    public abstract class LctAction
    {

    }
}
