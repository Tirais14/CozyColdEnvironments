#nullable enable
using System;
using System.Threading.Tasks;

namespace CCEnvs.Pools
{
    public interface IObjectPoolAsync<T> : IObjectPool
        where T : class
    {
        ValueTask<PooledHandle<T>> GetAsync();

        ValueTask PreheatAsync(int? count = null);

        void Return(T obj);
    }
}
