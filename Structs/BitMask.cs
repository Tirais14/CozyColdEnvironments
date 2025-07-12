#nullable enable
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UTIRLib
{
    public readonly struct BitMask : IReadOnlyList<bool>
    {
        private readonly BitArray bitArray;

        public readonly bool this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => bitArray[index];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => bitArray[index] = value;
        }

        public BitMask(int capacity)
        {
            bitArray = new BitArray(capacity);
        }

        public readonly IEnumerator<bool> GetEnumerator() => (IEnumerator<bool>)bitArray.GetEnumerator();

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        readonly int IReadOnlyCollection<bool>.Count => bitArray.Count;
    }
}
