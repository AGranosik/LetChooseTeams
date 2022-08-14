using LCT.Core.Entites.Tournaments.Events;

namespace LCT.Core.Entites
{
    public abstract class Aggregate<TKey>
    {
        private readonly IList<BaseEvent> _changes = new List<BaseEvent>();
        public long Version { get; private set; } = -1;
        public TKey Id { get; set; }
        protected abstract void When(object @event);
        public void Apply(BaseEvent @event)
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
        public BaseEvent[] GetChanges() => _changes.OrderBy(c => c.TimeStamp).ToArray();
    }
}
