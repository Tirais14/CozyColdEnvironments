using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs
{
    public class WeakLazy<T>
        where T : class
    {
        private readonly Func<T> factory;
        private WeakReference<T>? weakValue;

        public T Value => GetOrCreateValue();

        public WeakLazy(Func<T> factory)
        {
            Guard.IsNotNull(factory, nameof(factory));
            this.factory = factory;
        }

        private T GetOrCreateValue()
        {
            if (weakValue is null
                ||
                !weakValue.TryGetTarget(out T value)
                ||
                value.IsNull())
            {
                value = factory();

                if (weakValue is not null)
                    weakValue.SetTarget(value);
                else
                    weakValue = new WeakReference<T>(value);
            }

            return value;
        }
    }
}
