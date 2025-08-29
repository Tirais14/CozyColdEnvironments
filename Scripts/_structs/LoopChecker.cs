#nullable enable
namespace CozyColdEnvironments
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
                {
                    if (ExceptionMessage.IsNotNullOrEmpty())
                        throw new EndlessLoopException(iterations, ExceptionMessage);
                    else
                        throw new EndlessLoopException(iterations);
                }
                else
                    return false;
            }

            return condition;
        }
    }
}
