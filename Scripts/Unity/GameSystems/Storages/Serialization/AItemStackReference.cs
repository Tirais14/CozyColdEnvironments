using CCEnvs.Unity.EditorSerialization;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.GameSystems.Storages.Serialization
{
    [Serializable]
    public abstract class AItemStackReference : ITransformable<ItemStack>
    {
        [SerializeField]
        protected StringOrInteger itemKey;

        [SerializeField]
        protected int itemCount;

        public static implicit operator ItemStack(AItemStackReference source)
        {
            return source.DoTransform();
        }

        public abstract ItemStack DoTransform();
    }
}
