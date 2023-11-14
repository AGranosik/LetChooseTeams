namespace LCT.Domain.Common.Interfaces
{
    public interface IVersionable<TVersion>
    {
        int Version { get; }
        void VersionIncrement();
    }

    public interface IVersionable
    {
    }
}
