using System;
using System.Collections.Generic;
using CommunityToolkit.Diagnostics;

#nullable enable
namespace CCEnvs.Disposables
{
    public struct LightDisposable
        :
        IEquatable<LightDisposable>,
        IDisposable
    {
        public Action DisposeAction { get; }

        public LightDisposable(Action disposeAction)
            :
            this()
        {
            Guard.IsNotNull(disposeAction, nameof(disposeAction));

            DisposeAction = disposeAction;
        }

        public static bool operator ==(LightDisposable left, LightDisposable right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LightDisposable left, LightDisposable right)
        {
            return !(left == right);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is LightDisposable disposable && Equals(disposable);
        }

        public readonly bool Equals(LightDisposable other)
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

    public struct LightDisposable<TState> 
        :
        IEquatable<LightDisposable<TState>>,
        IDisposable
    {
        public TState State { get; }

        public Action<TState> DisposeAction { get; }

        public LightDisposable(TState state, Action<TState> disposeAction)
            :
            this()
        {
            Guard.IsNotNull(disposeAction, nameof(disposeAction));

            State = state;
            DisposeAction = disposeAction;
        }

        public static bool operator ==(LightDisposable<TState> left, LightDisposable<TState> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LightDisposable<TState> left, LightDisposable<TState> right)
        {
            return !(left == right);
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is LightDisposable<TState> disposable && Equals(disposable);
        }

        public readonly bool Equals(LightDisposable<TState> other)
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
