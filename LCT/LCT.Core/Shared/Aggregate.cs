namespace LCT.Core.Shared
{
    public abstract class Aggregate
    {
        private readonly IList<BaseEsEvent> _changes = new List<BaseEsEvent>();
        public long Version { get; private set; } = -1;
        protected abstract void When(object @event);
        public void Apply(BaseEsEvent @event)
        {
            When(@event);

            _changes.Add(@event);
        }

        public void Load(long version, IEnumerable<object> history)
        {
            Version = version;

            foreach (var item in history)
            {
                When(item);
            }
        }
        public BaseEsEvent[] GetChanges() => _changes.OrderBy(c => c.TimeStamp).ToArray();
    }
}
