using CCEnvs.Unity.EditorSerialization;
using System;
using UnityEngine;
using UnityEngine.Serialization;

#nullable enable
#pragma warning disable S1144
namespace CCEnvs.Unity.GameSystems.Storages.Serialization
{
    [Serializable]
    public abstract class AItemStackReference : ITransformable<ItemStack>
    {
        [field: SerializeField]
        public StringOrInteger ItemKey { get; private set; }

        [field: SerializeField]
        public int ItemCount { get; private set; }


        public static implicit operator ItemStack(AItemStackReference source)
        {
            return source.DoTransform();
        }

        public abstract ItemStack DoTransform();
    }
}
