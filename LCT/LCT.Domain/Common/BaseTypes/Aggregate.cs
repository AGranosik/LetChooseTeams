namespace LCT.Domain.Common.BaseTypes
{
    public abstract class Aggregate<TKey> : Entity<TKey>, IAgregateRoot
        where TKey : ValueType<TKey>
    {
        private readonly IList<DomainEvent> _changes = new List<DomainEvent>();
        protected Aggregate(TKey id) : base(id)
        {
        }

        protected abstract void When(object @event);
        public void Apply(DomainEvent @event)
        {
            When(@event);

            _changes.Add(@event);
        }

        public void Load(IEnumerable<object> history)
        {
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
    }

    public interface IAgregateRoot
    {
        void Load(IEnumerable<object> history);

        DomainEvent[] GetChanges();

        string AggregateId();
    }

}
