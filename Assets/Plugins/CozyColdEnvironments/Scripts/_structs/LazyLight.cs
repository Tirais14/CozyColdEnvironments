using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs
{
    public static class LazyLight
    {
        public static LazyLight<T> Create<T>()
            where T : new()
        {
            return new LazyLight<T>(() => new T()); 
        }
    }

    public ref struct LazyLight<T>
    {
        private readonly Func<T> factory;
        private T value;

        public T Value => GetValue();
        public bool HasValue { get; private set; }

        public LazyLight(Func<T> factory)
            :
            this()
        {
            Guard.IsNotNull(factory, nameof(factory));

            this.factory = factory;
            value = default!;
        }

        private T GetValue()
        {
            if (HasValue)
                return value;

            value = factory();
            HasValue = true;

            return value;
        }
    }

    public ref struct LazyLight<T, TState>
    {
        private readonly Func<TState, T> factory;
        private readonly TState state;
        private T value;

        public T Value => GetValue();
        public bool HasValue { get; private set; }  

        public LazyLight(TState state, Func<TState, T> factory)
            :
            this()
        {
            Guard.IsNotNull(factory, nameof(factory));

            this.state = state;
            this.factory = factory;
            value = default!;
        }

        private T GetValue()
        {
            if (HasValue)
                return Value;

            value = factory(state);
            HasValue = true;

            return value;
        }
    }
}
