#nullable enable
using CCEnvs.FuncLanguage;
using CommunityToolkit.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CCEnvs.Collections
{
    public struct AnonymousEnumerator<TTarget, TStateInternal, TItem> : IEnumerator<TItem>
    {
        private readonly Func<TTarget, Maybe<TStateInternal>, (TTarget target, TItem item, Maybe<TStateInternal> stateInternal, bool success)> moveNext;
        private readonly Func<TTarget, Maybe<TStateInternal>, (TTarget target, Maybe<TStateInternal> stateInternal)> reset;
        private readonly Action<TTarget, Maybe<TStateInternal>>? dispose;
        private TTarget target;
        private Maybe<TStateInternal> stateInternal;

        public TItem Current { get; private set; }

        readonly object IEnumerator.Current => Current!;

        public AnonymousEnumerator(
            TTarget target,
            Func<TTarget, Maybe<TStateInternal>, (TTarget target, TItem item, Maybe<TStateInternal> stateInternal, bool success)> moveNext,
            Func<TTarget, Maybe<TStateInternal>, (TTarget target, Maybe<TStateInternal> stateInternal)> reset,
            Action<TTarget, Maybe<TStateInternal>>? dispose = null) : this()
        {
            Guard.IsNotNull(moveNext, nameof(moveNext));
            Guard.IsNotNull(target, nameof(target));
            Guard.IsNotNull(reset, nameof(reset));

            this.target = target;
            this.moveNext = moveNext;
            this.reset = reset;
            this.dispose = dispose;
        }

        public bool MoveNext()
        {
            Validate();

            var (target, item, stateInternal, success) = moveNext(this.target, this.stateInternal);

            this.target = target;
            Current = item;
            this.stateInternal = stateInternal;

            return success;
        }

        public readonly void Reset()
        {
            Validate();
            reset(target, stateInternal);
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            Reset();
            dispose?.Invoke(target, stateInternal);

            disposed = true;
        }

        private readonly void Validate()
        {
            if (disposed)
                throw new ObjectDisposedException(GetType().ToString());
        }
    }
}
