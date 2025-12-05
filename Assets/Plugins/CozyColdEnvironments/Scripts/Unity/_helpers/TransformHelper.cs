using System;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable

namespace CCEnvs.Unity
{
    public static class TransformHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetParentCount(this Transform source)
        {
            CC.Guard.IsNotNull(source, nameof(source));

            int count = 0;
            Transform current = source;
            while (current != null)
            {
                count++;
                current = current.parent;
            }

            return count;
        }

        public static Vector3 Backward(this Transform value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.forward * -1;
        }

        public static Vector3 Left(this Transform value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.right * -1;
        }

        public static Vector3 Down(this Transform value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.up * -1;
        }
    }
}