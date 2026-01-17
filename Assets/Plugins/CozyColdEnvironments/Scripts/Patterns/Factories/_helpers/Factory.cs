using System;

#nullable enable
#pragma warning disable S2436
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
    }
}
