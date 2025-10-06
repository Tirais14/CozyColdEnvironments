#nullable enable
using System;

namespace CCEnvs.Diagnostics.Benchmarks
{
    public record BenchmarkResult : IBenchmarkTest
    {
        public TimeSpan Elapsed { get; set; }
        public int Repeats { get; set; } = 1;
        public string TestName { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"{nameof(TestName)}: {TestName} => {{{nameof(Repeats)}: {Repeats}, {nameof(Elapsed)}: {Elapsed.TotalMilliseconds} ms;}}.";
        }
    }
}
