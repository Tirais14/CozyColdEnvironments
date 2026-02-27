#nullable enable
using System.Collections.Generic;

namespace CCEnvs.Diagnostics.Benchmarks
{
    public record ForBenchmark<T> : CollectionBenchmark<T>
    {
        public ForBenchmark(IEnumerable<T> collection)
            :
            base(collection)
        {
            TestName = "For";
        }
    }
}
