#if MESSAGE_PIPE_PLUGIN

#nullable enable
using CommunityToolkit.Diagnostics;
using MessagePipe;
using System;

namespace CCEnvs.MPipe
{
    internal class PredicateFilter<TMessage, TState> : MessageHandlerFilter<TMessage>
    {
        private readonly Func<TMessage, TState, bool> predicate;
        private readonly TState state;

        public PredicateFilter(Func<TMessage, TState, bool> predicate, TState state)
        {
            Guard.IsNotNull(predicate);

            this.predicate = predicate;
            this.state = state;
        }

        public override void Handle(TMessage message, Action<TMessage> next)
        {
            if (predicate(message, state))
                next(message);
        }
    }
}
#endif