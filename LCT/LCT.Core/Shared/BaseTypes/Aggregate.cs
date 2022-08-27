namespace LCT.Core.Shared.BaseTypes
{
    public abstract class Aggregate<TKey> : Entity<TKey>, IAgregateRoot
        where TKey : ValueType<TKey>
    {
        private readonly IList<DomainEvent> _changes = new List<DomainEvent>();
        //event powinien byc wspolny dla Mediatr i tutaj
        // Zapis w repo triggeruje Event handler, ktory zarzadzi projekcję? a w przyszłości wrzuci to na kolejkę
        // w jakich warstwach??
        // eventy = domain
        // event handlery w application, tylko pytanie czy "domenowa obsługa" też powinno być w event handlerze czy już tylko ta applikacyjna
        protected Aggregate(TKey id) : base(id)
        {
        }

        protected abstract void When(object @event);
        public void Apply(DomainEvent @event)
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
        public DomainEvent[] GetChanges() => _changes
            .Where(c => c.Id == Guid.Empty)
            .OrderBy(c => c.TimeStamp)
            .ToArray();
    }

    public interface IAgregateRoot {
        void Load(IEnumerable<object> history);

        DomainEvent[] GetChanges();
    }

}
