using MediatR;

namespace LCT.Infrastructure.Events
{
    public abstract class LctAction<AggregateId> : LctAction, INotification
    {
        public AggregateId GroupKey { get; set; }
    }

    public abstract class LctAction
    {

    }
}
