#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Diagnostics;

namespace CCEnvs.Collections
{
    public static class ArraySegmentHelper
    {
        public static bool TryAddToArraySegment<T>(
            this ArraySegment<T?> segment,
            T item,
            [NotNullWhen(true)] out int? addedIdx)
        {
            CC.Guard.IsNotDefault(segment, nameof(segment));
            CC.Guard.IsNotNull(item, nameof(item));

            if (!segment.Array.TryFindHole(out var holeIdx))
            {
                addedIdx = null;
                return false;
            }

            segment.Array.AddToArray(item);
            addedIdx = holeIdx;
            return true;
        }

        public static int AddToArraySegment<T>(this ArraySegment<T?> segment, T item)
        {
            Guard.IsNotNull(segment, nameof(segment));
            CC.Guard.IsNotNull(item, nameof(item));

            if (!segment.Array.TryFindHole(out var holeIdx)
                ||
                holeIdx >= segment.Count)
            {
                throw new InvalidOperationException($"Cannot find hole of the {typeof(ArraySegment<T>)}");
            }

            return segment.Array.AddToArray(item);
        }
    }
}
