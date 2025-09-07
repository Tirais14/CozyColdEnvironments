#nullable enable

using CCEnvs.Async;

namespace CCEnvs
{
    public static class CC
    {
        public static AsyncTaskRegistry NeccesaryTasks { get; } = new();
        public static AsyncTaskRegistry BackgroundTasks { get; } = new();
        public static object EmptyObject { get; } = new object();
        public static string WordSeparator { get; set; } = "_";

        public static class C 
        {
            public static T[] Array<T>(params T[] values) => values;
        }
    }
}