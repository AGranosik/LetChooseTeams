namespace LCT.Infrastructure.Repositories
{
    public interface IRepository<T>
    {
        Task<T> Load(Guid Id);
        Task Save(T model);
    }
}
