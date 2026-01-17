#nullable enable
using System;

namespace CCEnvs.Pools
{
    public interface IObjectPool : IDisposable
    {
        int ActiveCount { get; }
        int InactiveCount { get; }
        int Count { get; }
        bool HasFactory { get; }

        void Clear();
    }

    public interface IObjectPool<T> : IObjectPool
        where T : class
    {
        PooledHandle<T> Get();

        void Return(T obj);
    }
}
