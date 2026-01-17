#nullable enable
using System;
using System.Threading.Tasks;

namespace CCEnvs.Pools
{
    public interface IObjectPoolAsync<T> : IDisposable
        where T : class
    {
        int ActiveCount { get; }
        int InactiveCount { get; }
        int Count { get; }

        ValueTask<PooledHandle<T>> GetAsync();

        void Return(T obj);

        void Clear();
    }
}
