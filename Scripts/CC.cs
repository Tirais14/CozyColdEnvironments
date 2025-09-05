#nullable enable

using CCEnvs.Async;
using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace CCEnvs
{
    public static class CC
    {
        public static AsyncTaskRegistry NeccesaryTasks { get; } = new();
        public static AsyncTaskRegistry BackgroundTasks { get; } = new();
        public static string WordSeparator { get; set; } = "_";

        public static class C 
        {
            public static T[] Array<T>(params T[] values) => values;
        }
    }
}