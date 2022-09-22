using MediatR;
using MongoDB.Bson.Serialization.Attributes;

namespace LCT.Application.Common.Events
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
