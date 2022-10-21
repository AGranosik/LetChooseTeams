namespace LCT.Domain.Common.BaseTypes
{
    public interface IVersionable<TVersion> : IVersionable
    {
        TVersion Version { get; }
    }

    public interface IVersionable
    {
        void Incerement();
    }
}
