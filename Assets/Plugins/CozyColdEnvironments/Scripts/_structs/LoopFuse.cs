using R3;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#nullable enable
#pragma warning disable IDE0251
namespace CCEnvs
{
    /// <summary>
    /// 
    /// </summary>
    public struct LoopFuse : IDisposable, IEquatable<LoopFuse>
    {
        public const int DEFAULT_ITERATION_LIMIT = 1000000;

        private ReactiveCommand<int>? onLimitReachedCmd;
        private bool isNotDefault;

        /// <summary>
        /// Triggered before the exception
        /// </summary>
        public event Action<int>? OnLimitReached;

        public int IterationPosition {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        public int IterationCount {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set;
        }

        public int IterationLimit {
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
            int iterationLimit = DEFAULT_ITERATION_LIMIT,
            bool throwOnLimitReached = true)
        {
            return new LoopFuse()
            {
                IterationLimit = iterationLimit,
                ThrowOnLimitReached = throwOnLimitReached,
                isNotDefault = true,
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
            if (!isNotDefault
                &&
                this.IsDefault())
            {
                IterationPosition = -1;
                IterationLimit = DEFAULT_ITERATION_LIMIT;
                ThrowOnLimitReached = true;

                isNotDefault = true;
            }

            IterationCount++;
            IterationPosition++;

            if (IterationCount > IterationLimit)
            {
                OnLimitReached?.Invoke(IterationPosition);
                onLimitReachedCmd?.Execute(IterationPosition);

                if (ThrowOnLimitReached)
                    throw CC.ThrowHelper.EndlessLoopException(IterationCount);

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

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            onLimitReachedCmd?.Dispose();

            disposed = true;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is LoopFuse fuse && Equals(fuse);
        }

        public readonly bool Equals(LoopFuse other)
        {
            return EqualityComparer<ReactiveCommand<int>?>.Default.Equals(onLimitReachedCmd, other.onLimitReachedCmd)
                   &&
                   IterationPosition == other.IterationPosition
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
                onLimitReachedCmd,
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

        /// <summary>
        /// Triggered before the exception
        /// </summary>
        public Observable<int> ObserveLimitReached()
        {
            if (this.IsDefault())
                return Observable.Empty<int>();

            onLimitReachedCmd ??= new ReactiveCommand<int>();

            return onLimitReachedCmd;
        }
    }
}
