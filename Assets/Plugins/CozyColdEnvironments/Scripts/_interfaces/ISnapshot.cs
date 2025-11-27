#nullable enable
namespace CCEnvs
{
    public interface ISnapshot
    {
        object Target { get; }

        void Restore();
    }
    public interface ISnapshot<out T> : ISnapshot
    {
        new T Target { get; }

        object ISnapshot.Target => Target!;
    }
}
