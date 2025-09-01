#nullable enable
using CCEnvs.Json.DTO;
using System;

#pragma warning disable S101
namespace CCEnvs.Json.DTO
{
    public interface ITypedJsonDTO : IJsonDto
    {
        public Type ObjectType { get; }
    }
}
