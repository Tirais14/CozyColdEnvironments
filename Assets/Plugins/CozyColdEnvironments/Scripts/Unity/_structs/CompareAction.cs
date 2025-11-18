using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace CCEnvs.Unity
{
    [Serializable]
    public struct CompareAction<T>
        where T : IComparable<T>
    {
        [UnityEngine.SerializeField]
        private T toCompare;

        [UnityEngine.SerializeField]
        private CompareTypes compareTypes;

        public CompareAction(T value, CompareTypes compareTypes)
        {
            toCompare = value;
            this.compareTypes = compareTypes;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Invoke(T other)
        {
            return toCompare.CompareTo(other, compareTypes);
        }
    }
}
