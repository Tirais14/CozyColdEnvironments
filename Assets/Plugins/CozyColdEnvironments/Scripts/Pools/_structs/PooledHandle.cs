using System;

#nullable enable
namespace CCEnvs.Pools
{
    public static class PooledHandle
    {
        public static PooledHandle<T> Create<T>(T value)
            where T : class
        {
            return new PooledHandle<T>(value);
        }

        public static PooledHandle<T> Create<T, TState>(T value, TState? state, Action<T, TState>? disposeAction)
            where T : class
        {
            return new PooledHandle<T>(value, state!, (value, state) => disposeAction?.Invoke(value, (TState)state));
        }
    }

    public struct PooledHandle<T> : IDisposable
        where T : class
    {
        public static PooledHandle<T> Default { get; } = new();

        private readonly Action<T, object>? disposeAction;

        public readonly object? State { get; }
        public readonly T Value { get; }
        public readonly bool IsValid { get; }

        public PooledHandle(T value)
            :
            this()
        {
            Value = value;

            IsValid = true;
        }

        public PooledHandle(T value, object? state, Action<T, object>? disposeAction)
            :
            this()
        {
            State = state;
            Value = value;
            this.disposeAction = disposeAction;

            IsValid = true;
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            try
            {
                if (State.IsNotNull() && disposeAction is not null)
                    disposeAction(Value, State);
            }
            finally
            {
                disposed = true;
            }

            disposed = true;
        }
    }
}
