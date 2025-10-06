#nullable enable
using System;

namespace CCEnvs.Diagnostics.Benchmarks
{
    public record BenchmarkTest : IBenchmarkTest
    {
        public const int DEFAULT_REPEATS = 1000000;

        private readonly BenchmarkResult inner = new();

        public event Action OnCompleted;

        public TimeSpan Elapsed => LastResult?.Elapsed ?? default;

        public int Repeats {
            get => inner.Repeats;
            set => inner.Repeats = value;
        }

        public string TestName {
            get => inner.TestName;
            set => inner.TestName = value;
        }

        public BenchmarkResult? LastResult { get; private set; }
        public bool PrintOnCompleted { get; set; }
        public Action Action { get; set; } = null!;

        public BenchmarkTest()
        {
            Repeats = DEFAULT_REPEATS;
        }

        public BenchmarkResult Run()
        {
            LastResult = Benchmark.Action(Action, Repeats, TestName);

            if (PrintOnCompleted)
                Benchmark.Print(LastResult);

            OnCompleted?.Invoke();

            return LastResult;
        }
    }
}
