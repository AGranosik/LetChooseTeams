using LCT.Core.Entites.Tournaments.Events;

namespace LCT.Core.Entites
{
    public abstract class Aggregate
    {
        private readonly IList<object> _changes = new List<object>();
        public Guid Id { get; protected set; } = Guid.Empty;
        public long Version { get; private set; } = -1;

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
        public object[] GetChanges() => _changes.ToArray();
    }
}
