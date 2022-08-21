using LCT.Core.Shared.BaseTypes;

namespace LCT.Infrastructure.Repositories
{
    public interface IRepository<TAggregateRoot>
        where TAggregateRoot : IAgregateRoot
    {
        Task<TAggregateRoot> LoadAsync(Guid Id); // id should be generic
        Task SaveAsync(TAggregateRoot model);
    }
}
