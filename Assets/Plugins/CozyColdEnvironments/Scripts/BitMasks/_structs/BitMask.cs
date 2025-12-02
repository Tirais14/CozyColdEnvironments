using System.Collections;
using UnityEngine;

#nullable enable
namespace CCEnvs
{
    public class BitMask
    {
        private BitArray bits;

        public BitMask(int capacity)
        {
            bits = new BitArray(capacity);
        }

        public BitMask(params bool[] bits)
        {
            this.bits = new BitArray(bits);
        }

        public void SetBit(int index)
        {
            //bits[index] = true;
            //bits.
        }
    }
}
