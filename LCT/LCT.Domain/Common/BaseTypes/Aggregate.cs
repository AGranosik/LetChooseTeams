using Newtonsoft.Json;

namespace LCT.Domain.Common.BaseTypes
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class Aggregate<TKey> : Entity<TKey>, IAgregateRoot
        where TKey : ValueType<TKey>
    {
        private readonly IList<DomainEvent> _changes = new List<DomainEvent>();
        [JsonProperty]
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

        public void Load(IEnumerable<DomainEvent> events)
        {
            _lastEventNumber ??= events.Max(d => d.EventNumber);
            foreach (var item in events)
            {
                When(item);
                SetEventNumber(item);
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
            var eventhasNumber = @event.EventNumber.HasValue;
            if (eventhasNumber)
            {
                _lastEventNumber = @event.EventNumber.Value > _lastEventNumber ? @event.EventNumber.Value : _lastEventNumber;
                return;
            }
            if (!_lastEventNumber.HasValue)
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
