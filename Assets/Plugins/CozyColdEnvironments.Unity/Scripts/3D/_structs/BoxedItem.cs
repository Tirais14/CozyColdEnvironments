using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.Pools;
using CCEnvs.Reflection.Caching;
using CCEnvs.Unity.Snapshots;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    public struct BoxedItem : IDisposable, IEquatable<BoxedItem>
    {
        private readonly PooledObject<RigidbodySnapshot> rigidbodySnapshotHandle;
        private readonly PooledObject<ColliderSnapshot> colliderSnapshotHandle;

        private bool isRestored;
        private bool isSetuped;

        public readonly IBoxItem Value { get; }

        public Vector3 Slot { get; }

        public ItemBox ItemBox { get; }

        private readonly RigidbodySnapshot? rigidbodySnapshot => rigidbodySnapshotHandle.Value;

        private readonly ColliderSnapshot colliderSnapshot => colliderSnapshotHandle.Value;

        public BoxedItem(ItemBox itemBox, IBoxItem value, Vector3 slot)
            :
            this()
        {
            CC.Guard.IsNotNull(itemBox, nameof(itemBox));
            CC.Guard.IsNotNull(value, nameof(value));

            var rb = value.rigidbody;

            if (rb != null)
                rigidbodySnapshotHandle = SnapshotPool<RigidbodySnapshot>.Shared.Get();

            colliderSnapshotHandle = SnapshotPool<ColliderSnapshot>.Shared.Get();

            Value = value;
            Slot = slot;
            ItemBox = itemBox;
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
                   EqualityComparer<RigidbodySnapshot?>.Default.Equals(rigidbodySnapshot, other.rigidbodySnapshot)
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

        internal readonly void MoveTo(Vector3 position)
        {
            var rb = Value.rigidbody;

            if (rb != null)
                rb.MovePosition(position);
            else
                Value.transform.position = position;
        }

        internal void Setup(
            bool collisionEnabled = false
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (isSetuped)
                return;

            var tr = Value.transform;
            tr.SetParent(ItemBox.cTransform);

            var rb = Value.rigidbody;
            if (rb != null && rigidbodySnapshot is not null)
            {
                SetupRigidbody();
                MoveRigidbodyAsync().Forget();
            }
            else
                MoveTransform();

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

        private readonly void SetupRigidbody()
        {
            var rb = Value.rigidbody;
            var snapshot = rigidbodySnapshot;

            if (snapshot == null || rb == null)
                return;

            snapshot.CaptureFrom(rb);
            snapshot.SetAngularVelocity(null)
                .SetLinearVelocity(null);

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        private readonly Vector3 GetWorldSlot()
        {
            var itemBox = ItemBox;
            var item = Value;
            var slot = Slot;

            var worldSlot = itemBox.cTransform.TransformPoint(slot);

            var itemOffset = item.sizeInfo.bounds.center - item.transform.position;

            var rotationPivot = worldSlot - itemOffset - itemBox.sizeInfo.bounds.center;

            var rotation = itemBox.cTransform.rotation * Quaternion.Euler(itemBox.ItemRotation);

            var rotatedSlot = rotation * rotationPivot + itemBox.sizeInfo.bounds.center + itemOffset;

            return rotatedSlot - itemOffset;
        }

        private readonly async UniTaskVoid MoveRigidbodyAsync()
        {
            try
            {
                await UniTask.WaitForFixedUpdate();

                var itemBox = ItemBox;
                var item = Value;
                var slot = Slot;

                if (!itemBox.ContainsItem(item)
                    ||
                    item.rigidbody == null)
                {
                    return;
                }

                var worldSlot = GetWorldSlot();

                item.rigidbody.Move(
                    worldSlot,
                    ItemBox.cTransform.rotation * Quaternion.Euler(ItemBox.ItemRotation)
                    );

                item.rigidbody.Sleep();
            }
            catch (Exception ex)
            {
                if (ex.IsCancellationException())
                    return;

                this.PrintException(ex);
            }
        }

        private readonly void MoveTransform()
        {
            var worldSlot = GetWorldSlot();
            var item = Value;

            item.transform.SetPositionAndRotation(worldSlot, ItemBox.cTransform.rotation * Quaternion.Euler(ItemBox.ItemRotation));
        }
    }
}
