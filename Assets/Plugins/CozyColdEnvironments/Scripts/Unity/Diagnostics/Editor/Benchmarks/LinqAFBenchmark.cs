#if LINQ_AF

using CCEnvs.Diagnostics.Benchmarks;
using LinqAF;
using System;
using System.Collections.Generic;

namespace CCEnvs.Unity.Diagnostics.EditorC.Benchmarks
{
    public record LinqAFBenchmark<T> : CollectionBenchmark<T>
    {
        public LinqAFBenchmark(IEnumerable<T> collection) : base(collection)
        {
            TestName = nameof(LinqAF);

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
#endif
