using System;

#nullable enable
namespace CCEnvs
{
    /// <summary>
    /// Use this for while cycle for preventing endless loop
    /// </summary>
    public class LoopFuse : ALoopFuse
    {
        public Func<bool> predicate;

        public LoopFuse(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        public bool Invoke()
        {
            if (!MoveNext())
                throw GetException();

            return predicate();
        }

        public static implicit operator bool(LoopFuse value) => value.Invoke();
    }

    public class LoopFuse<T> : ALoopFuse
    {
        private readonly Predicate<T> predicate;

        public LoopFuse(Predicate<T> predicate)
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

    public class LoopFuse<T0, T1> : ALoopFuse
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

    public class LoopFuse<T0, T1, T2> : ALoopFuse
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