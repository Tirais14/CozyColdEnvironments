#nullable enable
using System;

namespace UTIRLib
{
    public abstract class ALoopPredicate
    {
        private int iterations;

        /// <summary>
        /// Default = 100000
        /// </summary>
        public int IterationsLimit { get; set; } = 100000;
        public Exception Exception { get; set; } = new ExceptionPlaceholder("Endless cycle prevented.");

        protected bool MoveNext()
        {
            return iterations++ < IterationsLimit;
        }
    }
}
