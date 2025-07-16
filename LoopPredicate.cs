using System;

#nullable enable

namespace UTIRLib
{
    /// <summary>
    /// Use this for while cycle for preventing endless loop
    /// </summary>
    public class LoopPredicate : LoopPredicateBase
    {
        private readonly Func<bool> predicate;

        public LoopPredicate(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        public bool Invoke()
        {
            if (MoveNext())
                throw Exception;

            return predicate();
        }
    }

    public class LoopPredicate<T0> : LoopPredicateBase
    {
        private readonly Predicate<T0> predicate;

        public LoopPredicate(Predicate<T0> predicate)
        {
            this.predicate = predicate;
        }

        public bool Invoke(T0 value)
        {
            if (MoveNext())
                throw Exception;

            return predicate(value);
        }
    }

    public class LoopPredicate<T0, T1> : LoopPredicateBase
    {
        private readonly Func<T0, T1, bool> predicate;

        public LoopPredicate(Func<T0, T1, bool> predicate)
        {
            this.predicate = predicate;
        }

        public bool Invoke(T0 value, T1 value1)
        {
            if (MoveNext())
                throw Exception;

            return predicate(value, value1);
        }
    }

    public class LoopPredicate<T0, T1, T2> : LoopPredicateBase
    {
        private readonly Func<T0, T1, T2, bool> predicate;

        public LoopPredicate(Func<T0, T1, T2, bool> predicate)
        {
            this.predicate = predicate;
        }

        public bool Invoke(T0 value, T1 value1, T2 value2)
        {
            if (MoveNext())
                throw Exception;

            return predicate(value, value1, value2);
        }
    }
}