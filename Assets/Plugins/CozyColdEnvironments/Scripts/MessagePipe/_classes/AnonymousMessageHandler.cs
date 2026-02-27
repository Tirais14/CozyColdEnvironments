#if MESSAGE_PIPE_PLUGIN
using CommunityToolkit.Diagnostics;
using MessagePipe;
using System;

#nullable enable
namespace CCEnvs.MPipe
{
    internal class AnonymousMessageHandler<TMessage, TState> : IMessageHandler<TMessage>
    {
        private readonly Action<TMessage, TState> handler;
        private readonly TState state;

        public AnonymousMessageHandler(Action<TMessage, TState> handler, TState state)
        {
            Guard.IsNotNull(state);

            this.handler = handler;
            this.state = state;
        }

        public void Handle(TMessage message)
        {
            handler.Invoke(message, state);
        }
    }
}
#endif
