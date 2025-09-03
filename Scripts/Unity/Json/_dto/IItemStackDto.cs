using CCEnvs.Common;
using CCEnvs.Diagnostics;
using CCEnvs.Json.DTO;
using CCEnvs.Reflection;
using CCEnvs.Unity.GameSystems.Storages;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Runtime.Serialization;

#nullable enable
#pragma warning disable S101
namespace CCEnvs.Unity.Json
{
    [JsonObject]
    [Serializable]
    public record IItemStackDto
        : ItemStackDto,
        ITypedJsonDTO
    {
        [JsonProperty]
        public Type ObjectType { get; set; } = null!;

        public IItemStackDto()
        {
        }

        public IItemStackDto(IItemStack itemStack)
            :
            base(new ItemStack(itemStack))
        {
            ObjectType = itemStack.GetType();
        }

        [OnDeserialized]
        private void Validate(StreamingContext _)
        {
            if (ObjectType.IsNotType<IItemStack>())
                CCDebug.PrintException(new DataAccessException(ObjectType, nameof(ObjectType)));
        }
    }
    public record IItemStackDto<T> : IItemStackDto
        where T : IItemStack
    {
        public IItemStackDto()
        {
            ObjectType = typeof(T);
        }

        public IItemStackDto(T itemStack)
            :
            base(itemStack)
        {
            ObjectType = typeof(T);
        }
    }
}
