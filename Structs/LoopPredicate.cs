using System;

#nullable enable

namespace UTIRLib
{
    /// <summary>
    /// Use this for while cycle for preventing endless loop
    /// </summary>
    public ref struct LoopPredicate
    {
        private readonly Func<bool> func;
        private readonly int iterationsLimit;
        private readonly bool throwIfLimitReached;
        private readonly string throwMessage;
        private int iterations;

        public LoopPredicate(Func<bool> predicate,
                             int iterationsLimit = 10000,
                             bool throwIfLimitReached = true,
                             string throwMessage = "Endless cycle prevented.")
        {
            func = predicate;
            this.iterationsLimit = iterationsLimit;
            this.throwIfLimitReached = throwIfLimitReached;
            this.throwMessage = throwMessage;
            iterations = 0;
        }

        public bool Invoke()
        {
            if (iterations++ >= iterationsLimit)
            {
                if (throwIfLimitReached)
                    throw new Exception(throwMessage);
                else 
                    return false;
            }

            return func();
        }
    }
}