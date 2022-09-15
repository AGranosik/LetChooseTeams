namespace LCT.Infrastructure.Repositories.Actions
{
    public abstract class LctAction<AggregateId, ActionType>: LctAction
    {

        public AggregateId GroupKey { get; set; }
        public ActionType Type { get; set; }
    }

    public abstract class LctAction
    {

    }
}
