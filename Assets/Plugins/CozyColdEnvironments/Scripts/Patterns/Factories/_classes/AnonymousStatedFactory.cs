using CommunityToolkit.Diagnostics;
using System;

#nullable enable
namespace CCEnvs.Patterns.Factories
{
    public class AnonymousStatedFactory<TState, TOut> : IFactory<TOut>
    {
        private readonly Func<TState, TOut> factory;
        private readonly TState state;

        public AnonymousStatedFactory(TState state, Func<TState, TOut> factory)
        {
            CC.Guard.IsNotNullState(state);
            Guard.IsNotNull(factory, nameof(factory));

            this.state = state;
            this.factory = factory;
        }

        public TOut Create() => factory(state);
    }
}
