#nullable enable
using CCEnvs.Json.DTO;
using System;

#pragma warning disable S101
namespace CCEnvs.Json
{
    public interface ITypedJsonDTO : IJsonDTO
    {
        public Type ObjectType { get; }
    }
}
