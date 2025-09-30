using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CCEnvs.Diagnostics.Benchmarks
{
    public abstract record CollectionBenchmark<T> : BenchmarkTest
    {
        protected readonly T[] collection;
        protected readonly List<object> tempResults = new();
        protected object erased;

        public Func<T, object> OnEach { get; set; }

        protected CollectionBenchmark(IEnumerable<T> collection)
        {
            this.collection = collection.ToArray();

            OnEach = x => x.ToString();
            Action = () =>
            {
                for (int i = 0; i < this.collection.Length; i++)
                {
                    erased = OnEach(this.collection[i]);
                    tempResults.Add(this.collection[i]);
                }
            };
            OnCompleted += () => tempResults.Clear();
        }
    }
}
