#nullable enable
namespace UTIRLib.Tickables
{
    public interface IFixedTickable : ITickableBase
    {
        void DoFixedTick();
    }
    /// <summary>
    /// Generic parameter marks ticker in which to register
    /// </summary>
    /// <typeparam name="T">Marks ticker in which to register</typeparam>
    public interface IFixedTickable<in T> : IFixedTickable, ITickableBase<T>
        where T : ITicker
    {
    }
}
