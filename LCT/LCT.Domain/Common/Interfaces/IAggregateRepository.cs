using LCT.Domain.Common.BaseTypes;

namespace LCT.Domain.Common.Interfaces
{
    public interface IAggregateRepository<TAggregateRoot>
        where TAggregateRoot : IAgregateRoot
    {
        Task<TAggregateRoot> LoadAsync(Guid Id); // id should be generic
        Task SaveAsync(TAggregateRoot model);
    }
}
