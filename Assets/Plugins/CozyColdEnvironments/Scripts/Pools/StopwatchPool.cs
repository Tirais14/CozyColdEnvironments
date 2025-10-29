using Microsoft.Extensions.ObjectPool;
using System.Diagnostics;

#nullable enable
namespace CCEnvs.Pools
{
    public class StopwatchPool : DefaultObjectPool<Stopwatch>
    {
        public static StopwatchPool Instance { get; } = new(10);

        public StopwatchPool() : base(new Policy())
        {
        }

        public StopwatchPool(int maximumRetained) : base(new Policy(), maximumRetained)
        {
        }

        public readonly struct Policy : IPooledObjectPolicy<Stopwatch>
        {
            public Stopwatch Create() => new();

            public bool Return(Stopwatch obj)
            {
                CC.Guard.IsNotNull(obj, nameof(obj));

                obj.Stop();
                obj.Reset();

                return true;
            }
        }
    }
}
