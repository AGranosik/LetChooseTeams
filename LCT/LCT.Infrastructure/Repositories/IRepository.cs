namespace LCT.Infrastructure.Repositories
{
    public interface IRepository<T>
    {
        T Load(Guid Id);
        Task Save(T model);
    }
}
