using System;

#nullable enable
#pragma warning disable S2436
#pragma warning disable S3218
namespace CCEnvs.Patterns.Factories
{
    public static class Factory
    {
        public static IFactory<TOut> DefaultValueFactory<TOut>()
            where TOut : new()
        {
            return Create(() => new TOut());
        }

        public static IFactory<TOut> Create<TOut>(Func<TOut> factory)
        {
            return new AnonymousFactory<TOut>(factory);
        }

        public static IFactory<T, TOut> Create<T, TOut>(Func<T, TOut> factory)
        {
            return new AnonymousFactory<T, TOut>(factory);
        }

        public static IFactory<T, T1, TOut> Create<T, T1, TOut>(Func<T, T1, TOut> factory)
        {
            return new AnonymousFactory<T, T1, TOut>(factory);
        }

        public static IFactory<T, T1, T2, TOut> Create<T, T1, T2, TOut>(Func<T, T1, T2, TOut> factory)
        {
            return new AnonymousFactory<T, T1, T2, TOut>(factory);
        }

        public static IFactory<TOut> Create<TState, TOut>(TState state, Func<TState, TOut> factory)
        {
            return new AnonymousStatedFactory<TState, TOut>(state, factory);
        }

        public static class Async
        {
#if UNITASK_PLUGIN
            public static IFactory<Cysharp.Threading.Tasks.UniTask<TOut>> Create<TOut>(Func<Cysharp.Threading.Tasks.UniTask<TOut>> factory)
            {
                return new AnonymousFactory<Cysharp.Threading.Tasks.UniTask<TOut>>(factory);
            }

            public static IFactory<T, Cysharp.Threading.Tasks.UniTask<TOut>> Create<T, TOut>(Func<T, Cysharp.Threading.Tasks.UniTask<TOut>> factory)
            {
                return new AnonymousFactory<T, Cysharp.Threading.Tasks.UniTask<TOut>>(factory);
            }

            public static IFactory<T, T1, Cysharp.Threading.Tasks.UniTask<TOut>> Create<T, T1, TOut>(Func<T, T1, Cysharp.Threading.Tasks.UniTask<TOut>> factory)
            {
                return new AnonymousFactory<T, T1, Cysharp.Threading.Tasks.UniTask<TOut>>(factory);
            }

            public static IFactory<T, T1, T2, Cysharp.Threading.Tasks.UniTask<TOut>> Create<T, T1, T2, TOut>(Func<T, T1, T2, Cysharp.Threading.Tasks.UniTask<TOut>> factory)
            {
                return new AnonymousFactory<T, T1, T2, Cysharp.Threading.Tasks.UniTask<TOut>>(factory);
            }

            public static IFactory<Cysharp.Threading.Tasks.UniTask<TOut>> Create<TState, TOut>(TState state, Func<TState, Cysharp.Threading.Tasks.UniTask<TOut>> factory)
            {
                return new AnonymousStatedFactory<TState, Cysharp.Threading.Tasks.UniTask<TOut>>(state, factory);
            }

#else
            public static IFactory<System.Threading.Tasks.ValueTask<TOut>> Create<TOut>(Func<System.Threading.Tasks.ValueTask<TOut>> factory)
            {
                return new AnonymousFactory<System.Threading.Tasks.ValueTask<TOut>>(factory);
            }

            public static IFactory<T, System.Threading.Tasks.ValueTask<TOut>> Create<T, TOut>(Func<T, System.Threading.Tasks.ValueTask<TOut>> factory)
            {
                return new AnonymousFactory<T, System.Threading.Tasks.ValueTask<TOut>>(factory);
            }

            public static IFactory<T, T1, System.Threading.Tasks.ValueTask<TOut>> Create<T, T1, TOut>(Func<T, T1, System.Threading.Tasks.ValueTask<TOut>> factory)
            {
                return new AnonymousFactory<T, T1, System.Threading.Tasks.ValueTask<TOut>>(factory);
            }

            public static IFactory<T, T1, T2, System.Threading.Tasks.ValueTask<TOut>> Create<T, T1, T2, TOut>(Func<T, T1, T2, System.Threading.Tasks.ValueTask<TOut>> factory)
            {
                return new AnonymousFactory<T, T1, T2, System.Threading.Tasks.ValueTask<TOut>>(factory);
            }

            public static IFactory<System.Threading.Tasks.ValueTask<TOut>> Create<TState, TOut>(TState state, Func<TState, System.Threading.Tasks.ValueTask<TOut>> factory)
            {
                return new AnonymousStatedFactory<TState, System.Threading.Tasks.ValueTask<TOut>>(state, factory);
            }
#endif
        }
    }
}
