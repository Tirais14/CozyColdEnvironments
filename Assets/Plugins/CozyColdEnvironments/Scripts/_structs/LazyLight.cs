using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs
{
    public readonly ref struct LazyLight<T>
    {
        private readonly Func<T> factory;

        public readonly T Value => factory();

        public LazyLight(Func<T> factory)
        {
            Guard.IsNotNull(factory, nameof(factory));

            this.factory = factory;
        }
    }

    public readonly ref struct LazyLight<T, TState>
    {
        private readonly Func<TState, T> factory;
        private readonly TState state;

        public readonly T Value => factory(state);

        public LazyLight(Func<TState, T> factory, TState state)
        {
            Guard.IsNotNull(factory, nameof(factory));

            this.factory = factory;
            this.state = state;
        }
    }
}
