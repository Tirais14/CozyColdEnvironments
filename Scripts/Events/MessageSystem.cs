using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;

#nullable enable
namespace CCEnvs.Events
{
    public static class MessageSystem
    {
        private readonly static Dictionary<Type, object> messages = new();

        public static void Publish<T>(T message)
        {
            CC.Guard.NullArgument(message, nameof(message));

            var gType = typeof(T);
            if (!messages.ContainsKey(gType))
            {
                var observable = new AnonymousObservable<T>((obs) => Disposable.Create(message, static (msg) => messages.Remove(msg!.GetType())));
                messages.Add(gType, observable);
            }
        }

        public static IObservable<T> Recieve<T>()
        {
            return messages[typeof(T)].As<IObservable<T>>();
        }
    }
}
