using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Disposables
{
    public struct DisposableLight
        :
        IEquatable<DisposableLight>,
        IDisposable
    {
        public Action DisposeAction { get; }

        public DisposableLight(Action disposeAction)
            :
            this()
        {
            Guard.IsNotNull(disposeAction, nameof(disposeAction));

            DisposeAction = disposeAction;
        }

        public static bool operator ==(DisposableLight left, DisposableLight right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DisposableLight left, DisposableLight right)
        {
            return !(left == right);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is DisposableLight disposable && Equals(disposable);
        }

        public readonly bool Equals(DisposableLight other)
        {
            return DisposeAction == other.DisposeAction
                   &&
                   disposed == other.disposed;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(DisposeAction, disposed);
        }

        public readonly override string ToString()
        {
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(DisposeAction)}: {DisposeAction})";
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            DisposeAction?.Invoke();

            disposed = true;
        }
    }

    public struct DisposableLight<TState>
        :
        IEquatable<DisposableLight<TState>>,
        IDisposable
    {
        public TState State { get; }

        public Action<TState> DisposeAction { get; }

        public DisposableLight(TState state, Action<TState> disposeAction)
            :
            this()
        {
            Guard.IsNotNull(disposeAction, nameof(disposeAction));

            State = state;
            DisposeAction = disposeAction;
        }

        public static bool operator ==(DisposableLight<TState> left, DisposableLight<TState> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DisposableLight<TState> left, DisposableLight<TState> right)
        {
            return !(left == right);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is DisposableLight<TState> disposable && Equals(disposable);
        }

        public readonly bool Equals(DisposableLight<TState> other)
        {
            return EqualityComparer<TState>.Default.Equals(State, other.State)
                   &&
                   DisposeAction == other.DisposeAction
                   &&
                   disposed == other.disposed;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(State, DisposeAction, disposed);
        }

        public readonly override string ToString()
        {
            if (this == default)
                return StringHelper.EMPTY_OBJECT;

            return $"({nameof(State)}: {State}; {nameof(DisposeAction)}: {DisposeAction})";
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            DisposeAction?.Invoke(State);

            disposed = true;
        }
    }
}
