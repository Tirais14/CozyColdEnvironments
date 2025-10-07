using CCEnvs.Unity.EditorSerialization;
using System;
using UnityEngine;

#nullable enable
#pragma warning disable S1144
namespace CCEnvs.Unity
{
    [Serializable]
    public struct SerializedOption<T> : IEditorSerialized<T?>
    {
        [SerializeField]
        private bool hasValue;

        [SerializeField]
        private T? value;

        public readonly bool HasValue => hasValue;

        public readonly T? Value {
            get
            {
                if (!HasValue)
                    return default;

                return value;
            }
        }

        public SerializedOption(T? value, bool hasValue)
        {
            this.hasValue = hasValue;
            this.value = value;
        }

        public static implicit operator bool(SerializedOption<T> source)
        {
            return source.HasValue;
        }

        public static implicit operator T?(SerializedOption<T> source)
        {
            if (!source)
                return default;

            return source.Value;
        }
    }
}
