using CCEnvs.Diagnostics;
using Microsoft.Extensions.ObjectPool;
using System;

#nullable enable
namespace CCEnvs.Pools
{
    public struct Pooled<T> : IDisposable
        where T : class
    {
        public static Pooled<T> Default => new();

        private bool disposedValue;

        public readonly ObjectPool<T> Pool;

        public T Value { get; private set; }
        public readonly bool IsValid => !disposedValue && Pool is not null;

        public Pooled(ObjectPool<T> pool, T value)
            :
            this()
        {
            Pool = pool;
            Value = value;
        }

        public static implicit operator bool(Pooled<T> source)
        {
            return source.IsValid;
        }

        public void Dispose()
        {
            try
            {
                Pool.Return(Value);
                disposedValue = true;
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
        }
    }
}
