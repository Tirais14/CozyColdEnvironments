#nullable enable

using CCEnvs.Async;
using System;

namespace CCEnvs
{
    public static class CC
    {
        public static AsyncTaskRegistry NeccesaryTasks { get; } = new();
        public static AsyncTaskRegistry BackgroundTasks { get; } = new();
        public static object EmptyObject { get; } = new object();
        public static object[] EmptyArguments { get; } = Array.Empty<object>();
        public static string WordSeparator { get; set; } = "_";

        public static class C 
        {
            public static T[] Array<T>(params T[] values) => values;
        }
    }
}