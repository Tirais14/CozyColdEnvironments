using System;
using System.Collections.Generic;
using UnityEngine;
using UTIRLib.Diagnostics;
using UTIRLib.Utils;

#nullable enable

namespace UTIRLib.TwoD
{
    public abstract class CompositeCollider<TCollider> : MonoX, ICompositeCollider<TCollider>
        where TCollider : Collider2D
    {
        protected readonly List<Collider2D> overlaps = new();
        protected TCollider[] colliders = null!;
        protected int complexity;
        protected TCollider activeCollider = null!;

        public bool IsEnabled { get; }
        public int Complexity => complexity;
        public TCollider ActiveTrigger => activeCollider;

        public void Activate(int index)
        {
            activeCollider.IfNotNullQ((trigger) => trigger.enabled = false);
            activeCollider = colliders[index];
            Enable();
        }

        public T[] GetOverlaps<T>(ContactFilter2D? contactFilter = null, params T[] excludeObjs)
             where T : class
        {
            if (activeCollider == null) return Array.Empty<T>();

            List<T> results = new();
            Collider2DHelper.TryGetOverlaps(activeCollider, overlaps, results, contactFilter, excludeObjs);

            return results.ToArray();
        }

        public bool TryGetOverlaps<T>(List<T> results, ContactFilter2D? contactFilter = null, params T[] excludeObjs)
            where T : class
        {
            if (activeCollider == null) return false;

            return Collider2DHelper.TryGetOverlaps(activeCollider, overlaps, results, contactFilter, excludeObjs);
        }

        public void Enable() => activeCollider.IfNotNullQ((collider) => collider.enabled = true);

        public void Disable() => activeCollider.IfNotNullQ((collider) => collider.enabled = false);
    }
}