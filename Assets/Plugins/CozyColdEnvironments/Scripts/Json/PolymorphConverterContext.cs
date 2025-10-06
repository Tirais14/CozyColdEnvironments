using System;

#nullable enable
namespace CCEnvs.Json
{
    public record PolymorphConverterContext
    {
        public Type DeserializedType { get; }

        public PolymorphConverterContext(Type deserializedType)
        {
            DeserializedType = deserializedType;
        }
    }
}
