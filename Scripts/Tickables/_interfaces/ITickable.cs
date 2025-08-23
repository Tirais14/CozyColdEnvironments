#nullable enable
namespace UTIRLib.Tickables
{
    public interface ITickable : ITickableBase
    {
        void DoTick();
    }
    public interface ITickable<in T> : ITickable, ITickableBase<T>
        where T : ITicker
    {

    }
}
