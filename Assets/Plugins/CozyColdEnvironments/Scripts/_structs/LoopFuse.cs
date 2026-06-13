using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

#nullable enable
#pragma warning disable IDE0251
namespace CCEnvs
{
    /// <summary>
    /// 
    /// </summary>
    public struct LoopFuse : IEquatable<LoopFuse>
    {
        public const long DEFAULT_ITERATION_LIMIT = 1000000;

        private bool isInititalized;

        /// <summary>
        /// Triggered before the exception
        /// </summary>
        public event Action<long>? OnLimitReached;

        public long IterationPosition {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        public long IterationCount {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        public long IterationLimit {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LoopFuse Create(long iterationLimit = DEFAULT_ITERATION_LIMIT)
        {
            return new LoopFuse()
            {
                IterationLimit = iterationLimit,
                IterationPosition = -1,
                isInititalized = true,
            };
        }

        public static bool operator ==(LoopFuse left, LoopFuse right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LoopFuse left, LoopFuse right)
        {
            return !(left == right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNextThrow()
        {
            if (!MoveNextCore())
                throw CC.ThrowHelper.EndlessLoopException(IterationCount);

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => MoveNextCore();

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LoopFuse ResetIterationCount()
        {
            IterationCount = 0;

            return this;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is LoopFuse fuse && Equals(fuse);
        }

        public readonly bool Equals(LoopFuse other)
        {
            return IterationPosition == other.IterationPosition
                   &&
                   IterationCount == other.IterationCount
                   &&
                   IterationLimit == other.IterationLimit;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(
                IterationPosition,
                IterationCount,
                IterationLimit
                );
        }

        public readonly override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddProperty(nameof(IterationPosition), IterationPosition)
                .AddProperty(nameof(IterationCount), IterationCount)
                .AddProperty(nameof(IterationLimit), IterationLimit)
                .ToStringAndDispose();
        }

        private bool MoveNextCore()
        {
            if (!isInititalized)
            {
                IterationPosition = -1;
                IterationLimit = DEFAULT_ITERATION_LIMIT;

                isInititalized = true;
            }

            IterationCount++;
            IterationPosition++;

            return IterationCount <= IterationLimit;
        }
    }
}
