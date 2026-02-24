#if ZLINQ_PLUGIN
using CCEnvs.Diagnostics.Benchmarks;
using System.Collections.Generic;
using ZLinq;    

#nullable enable
namespace CCEnvs.Unity.Diagnostics.EditorC.Benchmarks
{
    public record ZLinqBenchmark<T> : CollectionBenchmark<T>
    {
        public ZLinqBenchmark(IEnumerable<T> collection) : base(collection)
        {
            TestName = nameof(ZLinq);

            Action = () =>
            {
                erased = collection.AsValueEnumerable()
                                   .Where(x => x.IsDefault() || x.IsNotDefault())
                                   .Select(OnEach)
                                   .Cast<object>()
                                   .ToArray();
            };
        }
    }
}
#endif //ZLINQ_PLUGIN
