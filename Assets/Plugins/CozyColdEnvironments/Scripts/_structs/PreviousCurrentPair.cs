#nullable enable
using System;
using System.Collections.Generic;

namespace CCEnvs
{
    public static class PreviousCurrentPair
    {
        public static PreviousCurrentPair<T> Create<T>(T previous, T current)
        {
            return new PreviousCurrentPair<T>(previous, current);
        }

        public static PreviousCurrentPair<TPrevious, TCurrent> CreateT<TPrevious, TCurrent>(TPrevious previous, TCurrent current)
        {
            return new PreviousCurrentPair<TPrevious, TCurrent>(previous, current);
        }
    }

    public readonly struct PreviousCurrentPair<T> : IEquatable<PreviousCurrentPair<T>>
    {
        public T Previous { get; }
        public T Current { get; }

        public PreviousCurrentPair(T previous, T current)
        {
            Previous = previous;
            Current = current;
        }

        public void Deconstruct(out T previous, out T current)
        {
            previous = Previous;
            current = Current;  
        }

        public static implicit operator PreviousCurrentPair<T>((T Previous, T Current) pair)
        {
            return new PreviousCurrentPair<T>(pair.Previous, pair.Current);
        }

        public static bool operator ==(PreviousCurrentPair<T> left, PreviousCurrentPair<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PreviousCurrentPair<T> left, PreviousCurrentPair<T> right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is PreviousCurrentPair<T> pair && Equals(pair);
        }

        public bool Equals(PreviousCurrentPair<T> other)
        {
            return EqualityComparer<T>.Default.Equals(Previous, other.Previous) &&
                   EqualityComparer<T>.Default.Equals(Current, other.Current);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Previous, Current);
        }

        public override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddProperty(nameof(Previous), Previous)
                .AddProperty(nameof(Current), Current)
                .ToStringAndDispose();
        }
    }

    public readonly struct PreviousCurrentPair<TPrevious, TCurrent> : IEquatable<PreviousCurrentPair<TPrevious, TCurrent>>
    {
        public TPrevious Previous { get; }
        public TCurrent Current { get; }

        public PreviousCurrentPair(TPrevious previous, TCurrent current)
        {
            Previous = previous;
            Current = current;
        }

        public void Deconstruct(out TPrevious previous, TCurrent current)
        {
            previous = Previous;
            current = Current;
        }

        public static implicit operator PreviousCurrentPair<TPrevious, TCurrent>((TPrevious Previous, TCurrent Current) pair)
        {
            return new PreviousCurrentPair<TPrevious, TCurrent>(pair.Previous, pair.Current);
        }

        public static bool operator ==(PreviousCurrentPair<TPrevious, TCurrent> left, PreviousCurrentPair<TPrevious, TCurrent> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PreviousCurrentPair<TPrevious, TCurrent> left, PreviousCurrentPair<TPrevious, TCurrent> right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is PreviousCurrentPair<TPrevious, TCurrent> pair && Equals(pair);
        }

        public bool Equals(PreviousCurrentPair<TPrevious, TCurrent> other)
        {
            return EqualityComparer<TPrevious>.Default.Equals(Previous, other.Previous) &&
                   EqualityComparer<TCurrent>.Default.Equals(Current, other.Current);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Previous, Current);
        }

        public override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddProperty(nameof(Previous), Previous)
                .AddProperty(nameof(Current), Current)
                .ToStringAndDispose();
        }
    }
}
