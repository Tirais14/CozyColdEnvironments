#nullable enable
using System.Threading;
using System.Threading.Tasks;

namespace CCEnvs.Pools
{
    public interface IObjectPoolAsync<T> : IObjectPoolBase<T>
        where T : class
    {
        ValueTask<PooledObject<T>> GetAsync(CancellationToken cancellationToken = default);
    }
}
