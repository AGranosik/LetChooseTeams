using MediatR;
using MongoDB.Bson.Serialization.Attributes;

namespace LCT.Infrastructure.Events
{
    public abstract class LctAction<AggregateId> : LctAction, INotification
    {
        [BsonId]
        public AggregateId GroupKey { get; set; }
    }

    public abstract class LctAction 
    {

    }
}
