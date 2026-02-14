#nullable enable
namespace CCEnvs.Pools
{
    public interface IObjectPool<T> : IObjectPoolBase<T>
        where T : class
    {
        PooledObject<T> Get();
    }
}
