#nullable enable
namespace CCEnvs
{
    public struct LoopChecker
    {
        private ulong iterations;

        public const ulong ITERATIONS_LIMIT_DEFAULT = 1000000;
        public bool ThrowOnLimit { get; set; }
        public ulong IterationsLimit { get; set; }
        public ulong Iteration { get; set; }
        public string? ExceptionMessage { get; set; }

        public bool MoveNext(bool condition)
        {
            iterations++;

            if (iterations > IterationsLimit)
            {
                if (ThrowOnLimit)
                    CC.Throw.EndlessLoop(iterations, ExceptionMessage);
                else
                    return false;
            }

            return condition;
        }
    }
}
