namespace LCT.Domain.Common.BaseTypes
{
    public interface IVersionable<TVersion> : IVersionable
    {
        TVersion Version { get; }
        void Incerement();
    }

    public interface IVersionable
    {
    }
}
