using System;

#nullable enable

namespace UTIRLib
{
    /// <summary>
    /// Use this for while cycle for preventing endless loop
    /// </summary>
    public struct LoopPredicate
    {
        private readonly Func<bool> func;
        private readonly int iterationsLimit;
        private readonly bool throwIfLimitReached;
        private int iterations;

        public LoopPredicate(Func<bool> predicate, int iterationsLimit = 10000, bool throwIfLimitReached = true)
        {
            func = predicate;
            this.iterationsLimit = iterationsLimit;
            this.throwIfLimitReached = throwIfLimitReached;
            iterations = 0;
        }

        public bool Invoke()
        {
            if (iterations++ >= iterationsLimit)
            {
                if (throwIfLimitReached)
                {
                    throw new Exception("Endless cycle prevented.");
                }
                else return false;
            }

            return func();
        }
    }
}