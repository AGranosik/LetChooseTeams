using LCT.Core.Shared.BaseTypes;

namespace LCT.Infrastructure.Repositories
{
    public interface IRepository<T, TKey>
        where T : Aggregate<TKey>, new()
        where TKey : ValueType<TKey>
    {
        Task<T> LoadAsync(Guid Id);
        Task SaveAsync(T model);
    }
}
