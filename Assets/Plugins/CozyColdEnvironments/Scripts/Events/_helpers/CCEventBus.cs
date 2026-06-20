using CommunityToolkit.Diagnostics;
using R3;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Events
{
    public static class CCEventBus
    {
        public static Observable<TEvent> Recieve<TEvent>() => Events<TEvent>.Emitter.Value;

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
            if (Events<TEvent>.Emitter.IsValueCreated)
                Events<TEvent>.Emitter.Value.Execute(new TEvent());

            if (Events<TEvent>.Actions.TryGetValue(out List<Action>? actions))
            {
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

                actions.Clear();
            }

            if (Events<TEvent>.EvActions.TryGetValue(out List<Action<TEvent>>? evActions))
            {
                for (int i = 0; i < evActions.Count; i++)
                {
                    try
                    {
                        evActions[i].Invoke(new TEvent());
                    }
                    catch (Exception ex)
                    {
                        typeof(CCEventBus).PrintException(ex);
                    }
                }

                evActions.Clear();
            }
        }
        public static void Publish<TEvent>(TEvent ev)
        {
            if (Events<TEvent>.Emitter.IsValueCreated)
                Events<TEvent>.Emitter.Value.Execute(ev);

            if (Events<TEvent>.EvActions.TryGetValue(out List<Action<TEvent>>? evActions))
            {
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

                evActions.Clear();
            }
        }

        private static class Events<TEvent>
        {
            public static Lazy<ReactiveCommand<TEvent>> Emitter { get; private set; } = new(() => new());

            public static Lazy<List<Action>> Actions { get; } = new(() => new());

            public static Lazy<List<Action<TEvent>>> EvActions { get; } = new(() => new());
        }
    }
}
