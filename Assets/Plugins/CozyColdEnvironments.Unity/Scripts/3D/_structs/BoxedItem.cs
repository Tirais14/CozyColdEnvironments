using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.Pools;
using CCEnvs.Reflection.Caching;
using CCEnvs.Unity.Snapshots;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    public partial struct BoxedItem : IDisposable, IEquatable<BoxedItem>
    {
        private readonly PooledObject<RigidBodySnapshot> rigidbodySnapshotHandle;
        private readonly PooledObject<ColliderSnapshot> colliderSnapshotHandle;

        private bool isRestored;
        private bool isSetuped;

        public readonly IBoxItem Value { get; }

        public Vector3 Slot { get; }

        private readonly RigidBodySnapshot? rigidbodySnapshot => rigidbodySnapshotHandle.Value;

        private readonly ColliderSnapshot colliderSnapshot => colliderSnapshotHandle.Value;

        public BoxedItem(IBoxItem value, Vector3 position)
            :
            this()
        {
            CC.Guard.IsNotNull(value, nameof(value));

            var rb = value.rigidbody;

            if (rb != null)
                rigidbodySnapshotHandle = SnapshotPool<RigidBodySnapshot>.Shared.Get();

            colliderSnapshotHandle = SnapshotPool<ColliderSnapshot>.Shared.Get();

            Value = value;
            Slot = position;
        }

        public static bool operator ==(BoxedItem left, BoxedItem right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BoxedItem left, BoxedItem right)
        {
            return !(left == right);
        }

        private bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;

            try
            {
                Restore();
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            rigidbodySnapshotHandle.Dispose();
            colliderSnapshotHandle.Dispose();

            disposed = true;
        }

        public readonly override string ToString()
        {
            if (Equals(default))
                return TypeCache<BoxedItem>.FullName;

            return $"({nameof(Value)}: {Value}; {nameof(Slot)}: {Slot})";
        }

        internal readonly void MoveTo(Vector3 position)
        {
            var rb = Value.rigidbody;

            if (rb != null)
                rb.MovePosition(position);
            else
                Value.transform.position = position;
        }

        internal void Setup(
            Transform parent,
            bool collisionEnabled = false
            )
        {   
            CCDisposable.ThrowIfDisposed(this, disposed);
            CC.Guard.IsNotNull(parent, nameof(parent));

            if (isSetuped)
                return;

            var tr = Value.transform;
            tr.SetParent(parent);

            var rb = Value.rigidbody;
            if (rb != null && rigidbodySnapshot is not null)
            {
                rigidbodySnapshot.CaptureFrom(rb);
                rigidbodySnapshot.SetAngularVelocity(null)
                    .SetLinearVelocity(null);

                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
                rb.useGravity = false;

                rb.MoveRotation(Quaternion.identity);

                rb.Sleep();
            }
            else
                tr.localRotation = Quaternion.identity;

            var col = Value.collider;
            colliderSnapshot.CaptureFrom(col);
            col.enabled = collisionEnabled;

            isSetuped = true;
            isRestored = false;
        }

        internal void Restore()
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (isRestored)
                return;

            var rb = Value.rigidbody;

            if (rb != null && rigidbodySnapshot is not null)
            {
                rigidbodySnapshot.TryRestore(rb);
                rb.WakeUp();
            }

            var col = Value.collider;
            colliderSnapshot.TryRestore(col);

            isRestored = true;
            isSetuped = false;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is BoxedItem item && Equals(item);
        }

        public readonly bool Equals(BoxedItem other)
        {
            return rigidbodySnapshotHandle.Equals(other.rigidbodySnapshotHandle)
                   &&
                   colliderSnapshotHandle.Equals(other.colliderSnapshotHandle)
                   &&
                   isRestored == other.isRestored
                   &&
                   isSetuped == other.isSetuped
                   &&
                   EqualityComparer<IBoxItem>.Default.Equals(Value, other.Value)
                   &&
                   Slot.Equals(other.Slot) 
                   &&
                   EqualityComparer<RigidBodySnapshot?>.Default.Equals(rigidbodySnapshot, other.rigidbodySnapshot)
                   &&
                   EqualityComparer<ColliderSnapshot>.Default.Equals(colliderSnapshot, other.colliderSnapshot)
                   &&
                   disposed == other.disposed;
        }

        public readonly override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(rigidbodySnapshotHandle);
            hash.Add(colliderSnapshotHandle);
            hash.Add(isRestored);
            hash.Add(isSetuped);
            hash.Add(Value);
            hash.Add(Slot);
            hash.Add(rigidbodySnapshot);
            hash.Add(colliderSnapshot);
            hash.Add(disposed);
            return hash.ToHashCode();
        }
    }
}
