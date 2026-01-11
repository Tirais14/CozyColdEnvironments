using CCEnvs.Collections;
using CommunityToolkit.Diagnostics;
using MessagePipe;
using System;

#nullable enable
namespace CCEnvs.MPipe
{
    public static class MessageHandlerHelper
    {
        public static IDisposable Subscribe<TMessage, TState>(
            this ISubscriber<TMessage> source,
            TState state,
            Action<TMessage, TState> handler,
            params MessageHandlerFilter<TMessage>[] filters)
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(handler);

            var handlerInst = new AnonymousMessageHandler<TMessage, TState>(handler, state);
            return source.Subscribe(handlerInst, filters);
        }

        public static IDisposable Subscribe<TMessage, TState>(
            this ISubscriber<TMessage> source,
            TState state,
            Action<TMessage, TState> handler,
            Func<TMessage, TState, bool> predicate,
            params MessageHandlerFilter<TMessage>[] filters)
        {
            CC.Guard.IsNotNullSource(source);
            Guard.IsNotNull(handler);
            Guard.IsNotNull(predicate);

            var predicateFilter = new PredicateFilter<TMessage, TState>(predicate, state);
            filters = filters.IsEmpty()
                ?
                new MessageHandlerFilter<TMessage>[] { predicateFilter }
                :
                filters.AppendArray(predicateFilter);

            return source.Subscribe(state, handler, filters);
        }
    }
}
