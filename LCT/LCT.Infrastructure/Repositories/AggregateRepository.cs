using EventStore.ClientAPI;
using LCT.Core.Entites;
using System.Text;
using System.Text.Json;

namespace LCT.Infrastructure.Repositories
{
    public class AggregateRepository
    {
        private readonly IEventStoreConnection _eventStore;

        public AggregateRepository(IEventStoreConnection eventStore)
        {
            _eventStore = eventStore;
        }

        // Zapisywanie eventów
        public async Task SaveAsync<T>(T aggregate) where T : Aggregate, new()
        {
            // Pobieramy zdarzenie danego agregatu i dokonujemy mapowania do klasy EventData
            // Wszelkie zdarzenia w EventStore przechowywane są w typie EventData
            // Parametr 1: identyfikator zdarzenia, Guid
            // Parametr 2: nazwa zdarzenia, np. TechItemCreated, TechItemMoved
            // Parametr 3: Określenie czy zdarzenie jest obiektem typu json
            // Parametr 4: Dane zdarzenia poddane serializacji ponieważ oczekiwany typ to tablica bajtów
            // Parametr 5: Metadane. Parametr może być null'em
            var events = aggregate.GetChanges()
                .Select(@event => new EventData(
                    Guid.NewGuid(),
                    @event.GetType().Name,
                    true,
                    Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event)),
                    Encoding.UTF8.GetBytes(@event.GetType().FullName)))
                .ToArray();

            if (!events.Any())
            {
                return;
            }

            // Nazwa strumienia: Zdarzenia danego agregata w EventStore nazywane są strumieniami
            // Przykładowa nazwa strumienia: TechItem-86f720f3-88e0-4145-91b5-19c7a765fa4a
            var streamName = GetStreamName(aggregate, aggregate.Id);

            // Dodanie eventa do EventStora
            var result = _eventStore.AppendToStreamAsync(streamName, ExpectedVersion.Any, events);
        }

        // Odczytywanie/pobieranie eventów
        public async Task<T> LoadAsync<T>(Guid aggregateId) where T : Aggregate, new()
        {
            if (aggregateId == Guid.Empty)
                throw new ArgumentException("Guid nie może być pusty", nameof(aggregateId));

            var aggregate = new T();

            // Określenie nazwy strumienia
            var streamName = GetStreamName(aggregate, aggregateId);

            var nextPageStart = 0L;

            do
            {
                // Pobieranie kolejnych zdarzeń z Event Stora w kolejności zgodnej z iteracją
                var page = await _eventStore.ReadStreamEventsForwardAsync(
                    streamName, nextPageStart, 4096, false);

                if (page.Events.Length > 0)
                {
                    // Wywołanie metody Load aggregatu, która pozwoli nam otrzymać ostateczną postać agregatu, tj.
                    // zebranie wszystkich eventów, które wystąpiły dla danego agregatu
                    aggregate.Load(
                        page.Events.Last().Event.EventNumber,
                        page.Events.Select(@event => JsonSerializer.Deserialize(Encoding.UTF8.GetString(@event.OriginalEvent.Data), Type.GetType(Encoding.UTF8.GetString(@event.OriginalEvent.Metadata)))
                        ).ToArray());
                }

                nextPageStart = !page.IsEndOfStream ? page.NextEventNumber : -1;
            } while (nextPageStart != -1);

            return aggregate;
        }

        private string GetStreamName<T>(T type, Guid aggregateId) => $"{type.GetType().Name}-{aggregateId}";
    }
}
