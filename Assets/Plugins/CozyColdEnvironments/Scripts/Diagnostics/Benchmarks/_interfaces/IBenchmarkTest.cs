#nullable enable
using System;

namespace CCEnvs.Diagnostics.Benchmarks
{
    public interface IBenchmarkTest
    {
        TimeSpan Elapsed { get; }
        int Repeats { get; }
        string TestName { get; }
    }
}
