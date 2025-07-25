#nullable enable
namespace UTIRLib
{
    public ref struct LoopChecker
    {
        public const ulong ITERATIONS_LIMIT_DEFAULT = 1000000;

        private readonly bool throwOnLimit;
        private readonly ulong iterationsLimit;
        private ulong iterations;

        public LoopChecker(bool throwOnLimit, ulong iterationsLimit = ITERATIONS_LIMIT_DEFAULT)
        {
            this.throwOnLimit = throwOnLimit;
            this.iterationsLimit = iterationsLimit;
            iterations = 0uL;
        }

        public bool MoveNext(bool condition)
        {
            iterations++;

            if (iterations >= iterationsLimit)
            {
                if (throwOnLimit)
                    throw new EndlessLoopException(iterations);
                else
                    return false;
            }

            return condition;
        }
    }
}
