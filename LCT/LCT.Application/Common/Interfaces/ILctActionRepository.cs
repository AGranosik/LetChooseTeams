using LCT.Application.Common.Events;

namespace LCT.Application.Common.Interfaces
{
    public interface ILctActionRepository<TLctAction>
        where TLctAction : LctAction
    {
        Task SaveAsync(TLctAction action);
    }
}
