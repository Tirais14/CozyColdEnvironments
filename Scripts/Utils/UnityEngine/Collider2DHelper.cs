using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UTIRLib.Collections;
using UTIRLib.Diagnostics;

namespace UTIRLib.Utils
{
    public static class Collider2DHelper
    {
#nullable enable

        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryGetOverlaps<T>(Collider2D collider, List<Collider2D> overlaps,
            List<T> results, ContactFilter2D? contactFilter = null, params T[] excludeObjs)
            where T : class
        {
            if (collider == null)
            {
                throw new ArgumentNullException(nameof(collider));
            }
            if (overlaps == null)
            {
                throw new ArgumentNullException(nameof(overlaps));
            }
            if (results == null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            return TryGetRawOverlaps(collider, contactFilter, overlaps) && TryProcceedOverlaps(overlaps, results, excludeObjs);
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryProcceedOverlaps<T>(List<Collider2D>? overlaps, List<T> results,
            params T[] excludeObjs)
            where T : class
        {
            if (excludeObjs.HasNullElement())
            {
                throw new WrongCollectionItemException();
            }
            if (overlaps == null)
            {
                return false;
            }
            if (results == null)
            {
                throw new ArgumentNullException(nameof(results));
            }

            int overlapsCount = overlaps!.Count;
            for (int i = 0; i < overlapsCount; i++)
            {
                if (overlaps[i].TryGetComponent<T>(out var result) && !excludeObjs.Contains(result))
                {
                    results.Add(result);
                }
            }

            return results.Count > 0;
        }

        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryGetRawOverlaps(Collider2D collider, ContactFilter2D? contactFilter,
            List<Collider2D> overlaps)
        {
            if (collider == null)
            {
                throw new ArgumentNullException(nameof(collider));
            }
            if (overlaps == null)
            {
                throw new ArgumentNullException(nameof(overlaps));
            }

            overlaps ??= new List<Collider2D>();
            if (overlaps.Count > 0)
            {
                overlaps.Clear();
            }

            if (contactFilter == null)
            {
                collider.Overlap(overlaps);
            }
            else
            {
                collider.Overlap(contactFilter.Value, overlaps);
            }

            return overlaps.Count > 0;
        }
    }
}