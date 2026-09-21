using CommunityToolkit.Diagnostics;
using System;
using System.Threading;

#nullable enable
namespace CCEnvs.Disposables
{
    public sealed class AnonymousDisposable : IDisposable
    {
        private readonly Action disposeAction;

        public AnonymousDisposable(Action disposeAction)
        {
            Guard.IsNotNull(disposeAction);
            this.disposeAction = disposeAction;
        }

        ~AnonymousDisposable() => Dispose();

        private int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            disposeAction();
            GC.SuppressFinalize(this);
        }
    }

    public sealed class AnonymousDisposable<TState> : IDisposable
    {
        private readonly Action<TState> disposeAction;
        private readonly TState state;

        public AnonymousDisposable(Action<TState> disposeAction, TState state)
        {
            Guard.IsNotNull(disposeAction);
            this.disposeAction = disposeAction;
            this.state = state;
        }

        ~AnonymousDisposable() => Dispose();

        private int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            disposeAction(state);
            GC.SuppressFinalize(this);
        }
    }
}
