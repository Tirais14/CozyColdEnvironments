using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Events
{
    public static class CCEventBus
    {
        public static Observable<TEvent> Recieve<TEvent>() => Events<TEvent>.Emitter;

        public static void RecieveAction<TEvent>(Action action)
        {
            Guard.IsNotNull(action);
            Events<TEvent>.Actions.Value.Add(action);
        }

        public static void RecieveActionWithEvent<TEvent>(Action<TEvent> action)
        {
            Guard.IsNotNull(action);
            Events<TEvent>.EvActions.Value.Add(action);
        }

        public static void Publish<TEvent>()
            where TEvent : new()
        {
            Events<TEvent>.Emitter.Execute(new TEvent());

            if (!Events<TEvent>.Actions.TryGetValue(out List<Action>? actions))
                return;

            for (int i = 0; i < actions.Count; i++)
            {
                try
                {
                    actions[i].Invoke();
                }
                catch (Exception ex)
                {
                    typeof(CCEventBus).PrintException(ex);
                }
            }
        }
        public static void Publish<TEvent>(TEvent ev)
        {
            Events<TEvent>.Emitter.Execute(ev);

            if (!Events<TEvent>.EvActions.TryGetValue(out List<Action<TEvent>>? evActions))
                return;

            for (int i = 0; i < evActions.Count; i++)
            {
                try
                {
                    evActions[i].Invoke(ev);
                }
                catch (Exception ex)
                {
                    typeof(CCEventBus).PrintException(ex);
                }
            }
        }

        private static class Events<TEvent>
        {
            public static ReactiveCommand<TEvent> Emitter { get; } = new();
            public static Lazy<List<Action>> Actions { get; } = new (() => new());
            public static Lazy<List<Action<TEvent>>> EvActions { get; } = new(() => new());
        }
    }
}
