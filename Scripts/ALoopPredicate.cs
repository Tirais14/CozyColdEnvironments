#nullable enable
namespace UTIRLib
{
    public abstract class ALoopPredicate
    {
        private ulong iterations;

        /// <summary>
        /// Default = 100000
        /// </summary>
        public ulong IterationsLimit { get; set; } = LoopChecker.ITERATIONS_LIMIT_DEFAULT;
        public string? ExceptionMessage { get; set; }

        protected bool MoveNext()
        {
            return iterations++ < IterationsLimit;
        }

        protected EndlessLoopException GetException()
        {
            if (ExceptionMessage.IsNullOrWhiteSpace())
                return new EndlessLoopException(iterations);
            else
                return new EndlessLoopException(iterations, ExceptionMessage);
        }
    }
}
