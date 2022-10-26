namespace LCT.Domain.Common.BaseTypes
{
    public interface IVersionable<TVersion> : IVersionable
    {
        int Version { get; }
        void Incerement();
    }

    public interface IVersionable
    {
    }
}
