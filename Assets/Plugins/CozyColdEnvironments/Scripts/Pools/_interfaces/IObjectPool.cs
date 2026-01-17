#nullable enable
namespace CCEnvs.Pools
{
    public interface IObjectPool<T>
        where T : class
    {
        void Release(T obj);

        PooledHandle<T> Get();
    }
}
