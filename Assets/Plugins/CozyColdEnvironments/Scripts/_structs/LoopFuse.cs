using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

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

        public bool ThrowOnLimitReached {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static LoopFuse Create(
            long iterationLimit = DEFAULT_ITERATION_LIMIT,
            bool throwOnLimitReached = true
            )
        {
            return new LoopFuse()
            {
                IterationLimit = iterationLimit,
                ThrowOnLimitReached = throwOnLimitReached,
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
        public bool MoveNext()
        {
            if (!isInititalized
                &&
                this.IsDefault())
            {
                IterationPosition = -1;
                IterationLimit = DEFAULT_ITERATION_LIMIT;
                ThrowOnLimitReached = true;

                isInititalized = true;
            }

            IterationCount++;
            IterationPosition++;

            if (IterationCount > IterationLimit)
            {
                OnLimitReached?.Invoke(IterationPosition);

                var ex = CC.ThrowHelper.EndlessLoopException(IterationCount);

                if (ThrowOnLimitReached)
                    throw ex;
                else
                    this.PrintException(ex);

                return false;
            }

            return true;
        }

        [DebuggerStepThrough]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Works only if setted CC_DEBUG preprocessor variable
        /// </summary>
        public bool DebugMoveNext()
        {
#if CC_DEBUG_ENABLED
            return MoveNext();
#else
            return true;
#endif
        }

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
                   IterationLimit == other.IterationLimit
                   &&
                   ThrowOnLimitReached == other.ThrowOnLimitReached;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(
                IterationPosition,
                IterationCount,
                IterationLimit,
                ThrowOnLimitReached
                );
        }

        public readonly override string ToString()
        {
            if (this.IsDefault())
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(IterationPosition)}: {IterationPosition}; {nameof(IterationLimit)}: {IterationLimit})";
        }
    }
}
