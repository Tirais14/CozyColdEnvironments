#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace CCEnvs.Services
{
    public class Service<T> : IDisposable
    {
        private T? value;

        public bool IsResolved { get; private set; }

        public Type Contract { get; }

        public object? ID { get; }

        public Service(object? id)
        {
            Contract = typeof(T);
            this.ID = id;
        }

        public T GetValue()
        {
            if (!IsResolved)
            {
                value = (T)CCServices.Resolve(Contract, ID)!;
                IsResolved = true;
                CCServices.UnregisterOnUnbindCallback(typeof(T), ResetValue, ID);
                CCServices.RegisterOnUnbindCallback(typeof(T), ResetValue, ID);
            }

            return value!;
        }

        public bool TryGetValue([NotNullWhen(true)] out T? result)
        {
            if (!IsResolved)
            {
                if (CCServices.TryResolveOut(Contract, out var tResult, ID))
                {
                    value = (T)tResult;
                    IsResolved = true;
                    CCServices.UnregisterOnUnbindCallback(typeof(T), ResetValue, ID);
                    CCServices.RegisterOnUnbindCallback(typeof(T), ResetValue, ID);
                }
            }

            result = value;
            return IsResolved;
        }

        private int disposed;
        public void Dispose()
        {
            if (Interlocked.Exchange(ref disposed, 1) != 0)
                return;

            CCServices.UnregisterOnUnbindCallback(typeof(T), ResetValue, ID);
        }

        private void ResetValue()
        {
            value = default;
            IsResolved = false;
        }
    }
}
