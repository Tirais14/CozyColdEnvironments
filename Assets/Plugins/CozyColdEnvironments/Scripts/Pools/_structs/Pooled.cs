using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Pools
{
    public struct Pooled<T> : IDisposable
        where T : class
    {
        public static Pooled<T> Default { get; } = new();

        private readonly object state;
        private readonly Action<object, T> disposeAction;

        public readonly T Value { get; }
        public readonly bool IsValid { get; }

        public Pooled(T value, object state, Action<object, T> disposeAction)
            :
            this()
        {
            Guard.IsNotNull(disposeAction, nameof(disposeAction));

            this.state = state;
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
                disposeAction(state, Value);
            }
            finally
            {
                disposed = true;
            }

            disposed = true;
        }
    }
}
