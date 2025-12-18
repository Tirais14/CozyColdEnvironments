using System;

#nullable enable
namespace CCEnvs.Json
{
    public readonly struct JsonPropertyInfo
    {
        public string Name { readonly get; init; }
        public Func<object, object?>? Get { readonly get; init; } 
        public Action<object, object?>? Set { readonly get; init; }
    }
}
