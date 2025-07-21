using System;

#nullable enable
namespace UTIRLib
{
    /// <summary>
    /// Use this for while cycle for preventing endless loop
    /// </summary>
    public class LoopPredicate : ALoopPredicate
    {
        public Func<bool> predicate;

        public LoopPredicate(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        public bool Invoke()
        {
            if (!MoveNext())
                throw GetException();

            return predicate();
        }
    }

    public class LoopPredicate<T> : ALoopPredicate
    {
        private readonly Predicate<T> predicate;

        public LoopPredicate(Predicate<T> predicate)
        {
            this.predicate = predicate;
        }

        public bool Invoke(T value)
        {
            if (!MoveNext())
                throw GetException();

            return predicate(value);
        }
    }

    public class LoopPredicate<T0, T1> : ALoopPredicate
    {
        public Func<T0, T1, bool> Predicate { get; set; } = null!;

        public bool Invoke(T0 value, T1 value1)
        {
            if (Predicate is null)
                throw new Exception($"{nameof(Predicate)} not setted.");

            if (!MoveNext())
                throw GetException();

            return Predicate(value, value1);
        }
    }

    public class LoopPredicate<T0, T1, T2> : ALoopPredicate
    {
        public Func<T0, T1, T2, bool> Predicate { get; set; } = null!;

        public bool Invoke(T0 value, T1 value1, T2 value2)
        {
            if (Predicate is null)
                throw new Exception($"{nameof(Predicate)} not setted.");

            if (!MoveNext())
                throw GetException();

            return Predicate(value, value1, value2);
        }
    }
}