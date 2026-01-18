#nullable enable
namespace CCEnvs.Pools
{
    public interface IObjectPoolAsync<T> : IObjectPool
        where T : class
    {
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<PooledHandle<T>>
#else
        System.Threading.Tasks.ValueTask<PooledHandle<T>>
#endif
            GetAsync();

#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask
#else
        System.Threading.Tasks.ValueTask
#endif
            PreheatAsync(int? count = null);

        void Return(T obj);
    }
}
