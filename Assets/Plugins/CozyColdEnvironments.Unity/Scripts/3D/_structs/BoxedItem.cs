using CCEnvs.Disposables;
using CCEnvs.Pools;
using CCEnvs.Reflection.Caching;
using CCEnvs.Unity.Snapshots;
using Generator.Equals;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    [Equatable]
    public partial struct BoxedItem<T> : IDisposable
        where T : IBoxItem
    {
        [DefaultEquality]
        private readonly PooledObject<RigidBodySnapshot> rigidbodySnapshotHandle;

        [DefaultEquality]
        private readonly PooledObject<ColliderSnapshot> colliderSnapshotHandle;

        [DefaultEquality]
        public readonly T Value { get; }

        [DefaultEquality]
        public Vector3 Slot { get; }

        [IgnoreEquality]
        private readonly RigidBodySnapshot? rigidbodySnapshot => rigidbodySnapshotHandle.Value;

        [IgnoreEquality]
        private readonly ColliderSnapshot colliderSnapshot => colliderSnapshotHandle.Value;

        public BoxedItem(T value, Vector3 position)
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

        public override string ToString()
        {
            if (Equals(default))
                return TypeCache<BoxedItem<T>>.FullName;

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

        private bool isSetuped;
        internal void Setup(
            bool collisionEnabled = false
            )
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (isSetuped)
                return;

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
            }

            var col = Value.collider;
            colliderSnapshot.CaptureFrom(col);
            col.enabled = collisionEnabled;

            isSetuped = true;
            isRestored = false;
        }

        private bool isRestored;
        internal void Restore()
        {
            CCDisposable.ThrowIfDisposed(this, disposed);

            if (isRestored)
                return;

            var rb = Value.rigidbody;

            if (rb != null && rigidbodySnapshot is not null)
                rigidbodySnapshot.TryRestore(rb);

            var col = Value.collider;
            colliderSnapshot.TryRestore(col);

            isRestored = true;
            isSetuped = false;
        }
    }
}
