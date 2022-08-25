namespace LCT.Core.Shared.BaseTypes
{
    public abstract class Aggregate<TKey> : Entity<TKey>, IAgregateRoot
        where TKey : ValueType<TKey>
    {
        private readonly IList<BaseEsEvent> _changes = new List<BaseEsEvent>();

        protected Aggregate(TKey id) : base(id)
        {
        }

        protected abstract void When(object @event);
        public void Apply(BaseEsEvent @event)
        {
            When(@event);

            _changes.Add(@event); // handle somehow domain events as well
        }

        public void Load(IEnumerable<object> history)
        {
            foreach (var item in history)
            {
                When(item);
            }
        }
        public BaseEsEvent[] GetChanges() => _changes.Where(c => c.Id == Guid.Empty).OrderBy(c => c.TimeStamp).ToArray();
    }

    public interface IAgregateRoot {
        void Load(IEnumerable<object> history);

        BaseEsEvent[] GetChanges();
    }

}
