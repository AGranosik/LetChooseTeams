using LCT.Core.Shared.BaseTypes;

namespace LCT.Infrastructure.Repositories
{
    public interface IRepository<TAggregateRoot>
        where TAggregateRoot : IAgregateRoot
    {
        Task<TAggregateRoot> LoadAsync(Guid Id);
        Task SaveAsync(TAggregateRoot model);
    }
}
