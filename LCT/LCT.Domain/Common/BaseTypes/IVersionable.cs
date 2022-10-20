namespace LCT.Domain.Common.BaseTypes
{
    public interface IVersionable<TVersion>
    {
        TVersion Version { get; }
        void Incerement();
    }
}
