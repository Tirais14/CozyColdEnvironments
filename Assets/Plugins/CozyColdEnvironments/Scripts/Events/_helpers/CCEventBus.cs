using R3;

#nullable enable
namespace CCEnvs.Events
{
    public static class CCEventBus
    {
        public static Observable<T> Recieve<T>() => Events<T>.Emitter;

        public static void Publish<T>()
            where T : new()
        {
            Publish(new T());
        }

        public static void Publish<T>(T ev)
        {
            Events<T>.Emitter.Execute(ev);
        }

        private static class Events<T>
        {
            public static ReactiveCommand<T> Emitter { get; } = new();
        }
    }
}
