namespace LCT.Domain.Common.BaseTypes
{
    public abstract class Aggregate<TKey> : Entity<TKey>, IAgregateRoot
        where TKey : ValueType<TKey>
    {
        private readonly IList<DomainEvent> _changes = new List<DomainEvent>();
        private int? _lastEventNumber = null;
        protected Aggregate(TKey id) : base(id)
        {
        }

        protected abstract void When(object @event);
        public void Apply(DomainEvent @event)
        {
            When(@event);
            SetEventNumber(@event);
            _changes.Add(@event);
        }

        public void Load(IEnumerable<DomainEvent> history)
        {
            _lastEventNumber = history.Max(d => d.EventNumber);
            foreach (var item in history)
            {
                When(item);
            }
        }
        public DomainEvent[] GetChanges() => _changes
            .Where(c => c.Id == Guid.Empty)
            .OrderBy(c => c.TimeStamp)
            .ToArray();

        public string AggregateId()
            => Id.ToString();

        private void SetEventNumber(DomainEvent @event)
        {
            if (!@event.EventNumber.HasValue && !_lastEventNumber.HasValue)
            {
                _lastEventNumber = _changes.Max(e => e.EventNumber) ?? 1;
            }
            else
            {
                _lastEventNumber++;
            }
            @event.EventNumber = _lastEventNumber;
        }
    }

    public interface IAgregateRoot
    {
        void Load(IEnumerable<DomainEvent> history);

        DomainEvent[] GetChanges();

        string AggregateId();
    }

}
