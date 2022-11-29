namespace LCT.Infrastructure.Persistance.EventsStorage.UniqnessFactories.Models
{
    public record AggregateVersionModel (string AggregateId, int Version);
}
