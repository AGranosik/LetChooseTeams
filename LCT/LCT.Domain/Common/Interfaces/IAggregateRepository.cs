using LCT.Domain.Common.BaseTypes;

namespace LCT.Domain.Common.Interfaces
{
    public interface IAggregateRepository<TAggregateRoot>
        where TAggregateRoot : IAgregateRoot
    {
        Task<TAggregateRoot> LoadAsync(Guid Id, CancellationToken cancellationToken = default(CancellationToken));
        Task SaveAsync(TAggregateRoot model, int version = 0, CancellationToken cancellationToken = default(CancellationToken)); 
    }
}
