#nullable enable
using CCEnvs.Json.DTO;
using System;

namespace CCEnvs.Json
{
    public record TypeProvider : ITypeProvider, IJsonDto
    {
        public Type ObjectType { get; }

        public TypeProvider(Type objectType)
        {
            ObjectType = objectType;
        }
    }
}
