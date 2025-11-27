using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable

namespace CCEnvs.Unity
{
    public static class TransformExtensions
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

        public static Transform? FindParent(this Transform transform, string n)
        {
            LoopFuse cyclePredicate = new(() => transform != null);
            do
            {
                transform = transform.parent;

                if (transform.name.Equals(n))
                    return transform;
            } while (cyclePredicate.Invoke());

            return null;
        }

        public static bool TryFindParent(this Transform transform, string n, [NotNullWhen(true)] out Transform? result)
        {
            result = transform.FindParent(n);

            return result != null;
        }

        public static bool TryFind(this Transform transform,
                                   string n,
                                   [NotNullWhen(true)] out Transform? result)
        {
            result = transform.Find(n);

            return result != null;
        }
    }
}