using System.Collections.Generic;
using System.Linq;

#pragma warning disable S1905
namespace CCEnvs.Diagnostics.Benchmarks
{
    public record SystemLinqBenchmark<T> : CollectionBenchmark<T>
    {
        public SystemLinqBenchmark(IEnumerable<T> collection) : base(collection)
        {
            TestName = new OperatorChain(nameof(System), nameof(System.Linq));

            Action = () =>
            {
                erased = collection.Where(x => x.IsDefault() || x.IsNotDefault())
                                   .Select(OnEach)
                                   .Cast<object>()
                                   .ToArray();
            };
        }
    }
}
