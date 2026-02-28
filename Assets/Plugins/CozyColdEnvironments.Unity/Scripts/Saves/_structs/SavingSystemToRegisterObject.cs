using CommunityToolkit.Diagnostics;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Saves
{
    [Serializable]
    public struct SavingSystemToRegisterObject : System.IEquatable<SavingSystemToRegisterObject>
    {
        [field: SerializeField]
        public UnityEngine.Object Object { get; private set; }

        [field: SerializeField]
        public string Key { get; private set; }

        public SavingSystemToRegisterObject(UnityEngine.Object @object, string key)
        {
            CC.Guard.IsNotNull(@object, nameof(@object));
            Guard.IsNotNullOrEmpty(key, nameof(key));

            Object = @object;
            Key = key;
        }

        public static bool operator ==(SavingSystemToRegisterObject left, SavingSystemToRegisterObject right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SavingSystemToRegisterObject left, SavingSystemToRegisterObject right)
        {
            return !(left == right);
        }

        public readonly override bool Equals(object obj)
        {
            return obj is SavingSystemToRegisterObject @object && Equals(@object);
        }

        public readonly bool Equals(SavingSystemToRegisterObject other)
        {
            return Object == other.Object
                   &&
                   Key == other.Key;
        }

        public readonly override int GetHashCode()
        {
            return System.HashCode.Combine(Object, Key);
        }
    }
}
