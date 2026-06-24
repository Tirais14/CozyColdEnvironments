using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs
{
    public static class LightLazy
    {
        public static LightLazy<T> CreateByNew<T>()
            where T : new()
        {
            return new LightLazy<T>(() => new T());
        }

        public static LightLazy<T> Create<T>(Func<T> factory)
        {
            return new LightLazy<T>(factory);
        }
    }

    public struct LightLazy<T>
    {
        private readonly Func<T>? factory;
        private T value;

        public T Value => GetValue();

        public bool IsValueCreated { get; private set; }

        public LightLazy(Func<T> factory)
            :
            this()
        {
            Guard.IsNotNull(factory, nameof(factory));

            this.factory = factory;
            value = default!;
        }

        public LightLazy(T value)
            :
            this()
        {
            IsValueCreated = true;
            this.value = value;
        }

        public T GetValue(T @default)
        {
            if (!IsValueCreated)
                return @default;

            return Value;
        }
        public TOut GetValue<TOut>(TOut @default)
        {
            if (!IsValueCreated)
                return @default;

            return Value.CastTo<TOut>();
        }

        public readonly bool TryGetValue(out T result)
        {
            if (!IsValueCreated)
            {
                result = default!;
                return false;
            }

            result = value;
            return true;
        }

        private T GetValue()
        {
            if (IsValueCreated)
                return value;

            if (factory is null)
                throw new InvalidOperationException("Factory not found");

            value = factory();
            IsValueCreated = true;

            return value;
        }
    }

    public ref struct LightLazy<T, TState>
    {
        private readonly Func<TState, T> factory;
        private readonly TState state;
        private T value;

        public T Value => GetValue();
        public bool IsValueCreated { get; private set; }

        public LightLazy(TState state, Func<TState, T> factory)
            :
            this()
        {
            Guard.IsNotNull(factory, nameof(factory));

            this.state = state;
            this.factory = factory;
            value = default!;
        }

        public T GetValue(T @default)
        {
            if (!IsValueCreated)
                return @default;

            return Value;
        }
        public TOut GetValue<TOut>(TOut @default)
        {
            if (!IsValueCreated)
                return @default;

            return Value.CastTo<TOut>();
        }

        public readonly bool TryGetValue(out T result)
        {
            if (!IsValueCreated)
            {
                result = default!;
                return false;
            }

            result = value;
            return true;
        }

        private T GetValue()
        {
            if (IsValueCreated)
                return value;

            value = factory(state);
            IsValueCreated = true;

            return value;
        }
    }
}
