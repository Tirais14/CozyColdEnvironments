#nullable enable
namespace UTIRLib
{
    public abstract class ALoopPredicate
    {
        private int iterations;

        /// <summary>
        /// Default = 100000
        /// </summary>
        public int IterationsLimit { get; set; } = 100000;
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
