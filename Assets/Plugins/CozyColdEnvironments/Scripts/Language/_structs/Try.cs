using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Language
{
    public readonly struct Try<T> : IEquatable<Try<T>>
    {
        public readonly T Value { get; }
        public readonly bool HasException => Exception is not null;
        public readonly Exception? Exception { get; }

        public Try(Func<T> valueGetter,
                   Action<T>? action = null,
                   Action<Exception>? onCatch = null,
                   Action? onFinally = null)
            :
            this()
        {
            CC.Guard.NullArgument(valueGetter, nameof(valueGetter));

            try
            {
                Value = valueGetter();
                action?.Invoke(Value);
            }
            catch (Exception ex)
            {
                Exception = ex;
                onCatch?.Invoke(ex);
            }
            finally
            {
                onFinally?.Invoke();
            }
        }

        public static implicit operator bool(Try<T> source)
        {
            return !source.HasException;
        }

        public static bool operator ==(Try<T> left, Try<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Try<T> left, Try<T> right)
        {
            return !(left == right);
        }

        public bool Equals(Try<T> other)
        {
            return EqualityComparer<T>.Default.Equals(Value, other.Value)
                   &&
                   Exception == other.Exception;
        }
        public override bool Equals(object obj)
        {
            return obj is Try<T> typed && Equals(typed);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, Exception);
        }
    }
}
