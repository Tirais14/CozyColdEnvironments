using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace UTIRLib.TwoD
{
    public interface ICompositeCollider : ISwitchable
    {
        public int Complexity { get; }

        public void Activate(int index);

        public T[] GetOverlaps<T>(ContactFilter2D? contactFilter = null, params T[] excludeObjs) where T : class;

        public bool TryGetOverlaps<T>(List<T> results, ContactFilter2D? contactFilter = null, params T[] excludeObjs) where T : class;
    }

    public interface ICompositeCollider<out TCollider> : ICompositeCollider
        where TCollider : Collider2D
    {
        public TCollider ActiveTrigger { get; }
    }

    public interface ICompositeTrigger<in TEnum, out TCollider> : ICompositeCollider<TCollider>
        where TEnum : Enum
        where TCollider : Collider2D
    {
        public void Activate(TEnum value);
    }
}