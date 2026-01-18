using System;
using System.Threading.Tasks;

#nullable enable
#pragma warning disable S2436
#pragma warning disable S3218
namespace CCEnvs.Patterns.Factories
{
    public static class Factory
    {
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
            public static IFactory<
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<TOut>
#else
        System.Threading.Tasks.ValueTask<TOut>
#endif
                > Create<TOut>(Func<
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<TOut>
#else
        System.Threading.Tasks.ValueTask<TOut>
#endif
                > factory)
            {
                return new AnonymousFactory<
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<TOut>
#else
        System.Threading.Tasks.ValueTask<TOut>
#endif
                    >(factory);
            }

            public static IFactory<T,
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<TOut>
#else
        System.Threading.Tasks.ValueTask<TOut>
#endif
                > Create<T, TOut>(Func<T,
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<TOut>
#else
        System.Threading.Tasks.ValueTask<TOut>
#endif
                    > factory)
            {
                return new AnonymousFactory<T,
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<TOut>
#else
        System.Threading.Tasks.ValueTask<TOut>
#endif
                    >(factory);
            }

            public static IFactory<T, T1,
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<TOut>
#else
        System.Threading.Tasks.ValueTask<TOut>
#endif
                > Create<T, T1, TOut>(Func<T, T1,
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<TOut>
#else
        System.Threading.Tasks.ValueTask<TOut>
#endif
                    > factory)
            {
                return new AnonymousFactory<T, T1,
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<TOut>
#else
        System.Threading.Tasks.ValueTask<TOut>
#endif
                    >(factory);
            }

            public static IFactory<T, T1, T2,
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<TOut>
#else
        System.Threading.Tasks.ValueTask<TOut>
#endif
                > Create<T, T1, T2, TOut>(Func<T, T1, T2,
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<TOut>
#else
        System.Threading.Tasks.ValueTask<TOut>
#endif
                    > factory)
            {
                return new AnonymousFactory<T, T1, T2,
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<TOut>
#else
        System.Threading.Tasks.ValueTask<TOut>
#endif
                    >(factory);
            }

            public static IFactory<
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<TOut>
#else
        System.Threading.Tasks.ValueTask<TOut>
#endif
                > Create<TState, TOut>(TState state, Func<TState,
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<TOut>
#else
        System.Threading.Tasks.ValueTask<TOut>
#endif
                    > factory)
            {
                return new AnonymousStatedFactory<TState,
#if UNITASK_PLUGIN
        Cysharp.Threading.Tasks.UniTask<TOut>
#else
        System.Threading.Tasks.ValueTask<TOut>
#endif
                    >(state, factory);
            }
        }
    }
}
