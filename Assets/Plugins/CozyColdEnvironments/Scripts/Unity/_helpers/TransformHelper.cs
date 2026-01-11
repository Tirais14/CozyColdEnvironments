using CCEnvs.Collections;
using Cysharp.Text;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using ZLinq;

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

        public static HierarchyPath GetHierarchyPath(this Transform source)
        {
            CC.Guard.IsNotNullSource(source);

            if (source.parent == null)
                return new HierarchyPath(source.name, source.GetSiblingIndex());

            var parents = Do.Collect(source, (x) => x.parent);

            using var pathBuilder = ZString.CreateStringBuilder();

            pathBuilder.Grow(parents.Count);
            pathBuilder.AppendJoin("/", parents.AsValueEnumerable().Reverse().Select(x => x.name).AsEnumerable());

            return new HierarchyPath(pathBuilder.ToString(), source.GetSiblingIndex());
        }

        public static bool MatchHierarchyPath(this Transform source, HierarchyPath hierarchyPath)
        {
            CC.Guard.IsNotNullSource(source);
            return source.GetHierarchyPath() == hierarchyPath;
        }
    }
}