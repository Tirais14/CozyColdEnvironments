#nullable enable
namespace CCEnvs.Pools
{
    public interface IObjectPoolAsync<T> : IObjectPoolBase<T>
        where T : class
    {
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<PooledHandle<T>>
#else
        System.Threading.Tasks.Task<PooledHandle<T>>
#endif
            GetAsync();

#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask
#else
        System.Threading.Tasks.Task
#endif
            PreheatAsync(int? count = null, int? batchSize = null);
    }
}
