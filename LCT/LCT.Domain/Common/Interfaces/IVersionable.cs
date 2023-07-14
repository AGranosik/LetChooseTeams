namespace LCT.Domain.Common.Interfaces
{
    public interface IVersionable<TVersion> : IVersionable
    {
        int Version { get; }
        void VersionIncrement();
    }

    public interface IVersionable
    {
    }
}
