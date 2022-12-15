using LCT.Application.Common.Events;

namespace LCT.Infrastructure.Persistance.ActionsStorage
{
    public interface IActionStorageClient
    {
        Task<List<T>> GetActionsAsync<T, TKey>(TKey aggregateId)
             where T : LctAction<TKey>;

        Task SaveActionAsync<T>(T action) where T : LctAction;
    }
}
