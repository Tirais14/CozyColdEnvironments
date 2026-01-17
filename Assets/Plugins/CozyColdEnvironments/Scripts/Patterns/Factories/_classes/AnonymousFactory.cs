using CommunityToolkit.Diagnostics;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Patterns.Factories
{
    public class AnonymousFactory<TOut> : IFactory<TOut>
    {
        private readonly Func<TOut> factory;

        public AnonymousFactory(Func<TOut> factory)
        {
            Guard.IsNotNull(factory, nameof(factory));

            this.factory = factory;
        }

        public TOut Create() => factory();
    }

    public class AnonymousFactory<T, TOut> : IFactory<T, TOut>
    {
        private readonly Func<T, TOut> factory;

        public AnonymousFactory(Func<T, TOut> factory)
        {
            Guard.IsNotNull(factory, nameof(factory));

            this.factory = factory;
        }

        public TOut Create(T arg) => factory(arg);
    }

    public class AnonymousFactory<T, T1, TOut> : IFactory<T, T1, TOut>
    {
        private readonly Func<T, T1, TOut> factory;

        public AnonymousFactory(Func<T, T1, TOut> factory)
        {
            Guard.IsNotNull(factory, nameof(factory));

            this.factory = factory;
        }

        public TOut Create(T arg, T1 arg1) => factory(arg, arg1);
    }

    public class AnonymousFactory<T, T1, T2, TOut> : IFactory<T, T1, T2, TOut>
    {
        private readonly Func<T, T1, T2, TOut> factory;

        public AnonymousFactory(Func<T, T1, T2, TOut> factory)
        {
            Guard.IsNotNull(factory, nameof(factory));

            this.factory = factory;
        }

        public TOut Create(T arg, T1 arg1, T2 arg2) => factory(arg, arg1, arg2);
    }
}
