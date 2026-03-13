using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Saves
{
    public readonly struct WriteEventInfo<T> : IEquatable<WriteEventInfo<T>> where T : class
    {
        public T Target { get; }

        public WriteSaveDataMode WriteMode { get; }

        public WriteEventInfo(T target, WriteSaveDataMode writeMode)
        {
            CC.Guard.IsNotNull(target, nameof(target));

            Target = target;
            WriteMode = writeMode;
        }

        public static bool operator ==(WriteEventInfo<T> left, WriteEventInfo<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(WriteEventInfo<T> left, WriteEventInfo<T> right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is WriteEventInfo<T> info && Equals(info);
        }

        public bool Equals(WriteEventInfo<T> other)
        {
            return EqualityComparer<T>.Default.Equals(Target, other.Target)
                   &&
                   WriteMode == other.WriteMode;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Target, WriteMode);
        }

        public override string ToString()
        {
            return $"({nameof(Target)}: {Target}; {nameof(WriteMode)}: {WriteMode})";
        }
    }
}
