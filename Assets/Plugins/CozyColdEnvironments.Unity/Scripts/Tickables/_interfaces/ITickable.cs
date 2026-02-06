#nullable enable
namespace CCEnvs.Unity.Tickables
{
    public interface ITickable : ITickableBase
    {
        void DoTick();
    }
    /// <summary>
    /// Generic parameter marks ticker in which to register
    /// </summary>
    /// <typeparam name="T">Marks ticker in which to register</typeparam>
    public interface ITickable<in T> : ITickable, ITickableBase<T>
        where T : ITicker
    {
    }
}
