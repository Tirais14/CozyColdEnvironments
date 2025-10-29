using System;
using System.Diagnostics;

namespace CCEnvs.Diagnostics.Benchmarks
{
    public static class Benchmark
    {
        public static BenchmarkResult Action(Action action, int repeats = 1, string testName = "")
        {
            CC.Guard.IsNotNull(action, nameof(action));

            if (repeats < 1)
                repeats = 1;

            action(); //preheat

            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < repeats; i++)
                action();
            sw.Stop();

            return new BenchmarkResult
            {
                Elapsed = sw.Elapsed / repeats,
                Repeats = repeats,
                TestName = testName
            };
        }

        public static void Print(BenchmarkResult benchmarkResult)
        {
            CC.Guard.IsNotNull(benchmarkResult, nameof(benchmarkResult));

            typeof(Benchmark).PrintLog(benchmarkResult);
        }
    }
}
