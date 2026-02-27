#nullable enable
using System.Threading;

namespace CCEnvs.Pools
{
    public interface IObjectPoolAsync<T> : IObjectPoolBase<T>
        where T : class
    {
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<PooledObject<T>>
#else
        System.Threading.Tasks.ValueTask<PooledObject<T>>
#endif
            GetAsync(CancellationToken cancellationToken = default);
    }
}
