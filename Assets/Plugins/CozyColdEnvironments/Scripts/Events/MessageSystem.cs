using R3;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Events
{
    public static class MessageSystem
    {
        private readonly static Dictionary<Type, object> messages = new();

        public static void Publish<T>(T message)
        {
            CC.Guard.IsNotNull(message, nameof(message));

            var gType = typeof(T);
            if (!messages.ContainsKey(gType))
            {
                var observable = Observable.Create<T>((obs) =>
                {
                    return Disposable.Create(message, static (msg) => messages.Remove(msg!.GetType()));
                });

                messages.Add(gType, observable);
            }
        }

        public static Observable<T> Recieve<T>()
        {
            return messages[typeof(T)].To<Observable<T>>();
        }
    }
}
