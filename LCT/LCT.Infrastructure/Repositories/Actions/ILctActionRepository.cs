using LCT.Infrastructure.Events;

namespace LCT.Infrastructure.Repositories.Actions
{
    public interface ILctActionRepository<TLctAction>
        where TLctAction : LctAction
    {
        Task SaveAsync(TLctAction action);
    }
}
