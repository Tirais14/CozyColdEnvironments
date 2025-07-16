#nullable enable
using System;

namespace UTIRLib
{
    public abstract class LoopPredicateBase
    {
        private readonly int iterationsLimit;
        private int iterations;

        public int IterationsLimit { get; set; } = 100000;
        public Exception Exception { get; set; } = new ExceptionPlaceholder("Endless cycle prevented.");

        protected bool MoveNext()
        {
            return iterations++ < iterationsLimit;
        }
    }
}
