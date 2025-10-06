#nullable enable
using System;
using System.Collections.Generic;

namespace CCEnvs.Json
{
    public static class JsonSerializerCache
    {
        public static HashSet<Type> AsJsonCacheable { get; } = new(0);
        public static Dictionary<Type, object> Objects { get; } = new(0);
    }
}
