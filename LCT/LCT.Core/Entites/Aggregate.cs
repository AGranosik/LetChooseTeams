namespace LCT.Core.Entites
{
    public abstract class Aggregate
    {
        // Defincja listy w której będziemy przechowywać agregaty
        readonly IList<object> _changes = new List<object>();

        // Agregat będzie skadał się z unikalnego identyfikatora
        public Guid Id { get; protected set; } = Guid.Empty;
        // Agregat będzie wersjonowany
        public long Version { get; private set; } = -1;

        protected abstract void When(object @event);

        // Metoda, która doda event do naszej kolekcji
        // @ - znak pozwala używać zarezerwowanych nazw parametrów
        public void Apply(object @event)
        {
            When(@event);

            _changes.Add(@event);
        }

        // Metoda, która będzie dodawała zdarzenia do agregatu
        // Ostateczna wersja agregatu będzie tworzona przez uruchomienie tej metody dla
        // każdego zdarzenia odczytanego z Event Stora
        public void Load(long version, IEnumerable<object> history)
        {
            Version = version;

            foreach (var item in history)
            {
                When(item);
            }
        }

        // Metoda, która zwraca zdarzenia, które wystąpiły na danym agregacie
        // Metoda ta będzie uruchamiana w trakcie wysyłania zdarzeń do Event Stora
        // Eventy będą odbierane i dodawane do naszej kolekcji
        public object[] GetChanges() => _changes.ToArray();
    }
}
