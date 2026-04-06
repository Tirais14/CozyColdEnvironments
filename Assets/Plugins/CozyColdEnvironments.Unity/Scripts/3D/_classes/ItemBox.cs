using CCEnvs;
using CCEnvs.Collections;
using CCEnvs.Disposables;
using CCEnvs.Events;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.D3.Events;
using CCEnvs.Unity.EditorSerialization;
using CCEnvs.Unity.Injections;
using R3;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    [RequireComponent(typeof(Collider), typeof(ExternalSize))]
    public class ItemBox: CCBehaviour, IEnumerable<BoxedItem>
    {
        public const int EMPTY_POSITION_SEARCH_STEP_COUNT_MIN = 1;

        [Header("Packed Item")]
        [Space(6f)]

        [SerializeField]
        protected Vector3 itemRotation;

        [Header("Spawn")]
        [Space(6f)]

        [SerializeField]
        protected SerializedNullable<LayerMask> spawnObstaclesMask;

        [SerializeField, Min(EMPTY_POSITION_SEARCH_STEP_COUNT_MIN)]
        protected int spawnPointSeacrhSteps = 1;

        [SerializeField]
        protected Axes spawnPointSearchExcludeAxes = Axes.None;

        private readonly List<BoxedItem> items = new();
        private readonly List<Bounds> resolvedSlotsBuffer = new();

        private readonly C5.IntervalHeap<Vector3> emptySlots = new(Comparer<Vector3>.Create((left, right) => left.y.CompareTo(right.y)));

        private readonly HashSet<IBoxItem> pushedItems = new();

        private Vector3? itemSlotExtents;

        private ReactiveCommand<BoxedItem>? onPushedItem;
        private ReactiveCommand<IBoxItem>? onSpawnedItem;
        private ReactiveCommand<IBoxItem>? onPoppedItem;

        public IReadOnlyCollection<BoxedItem> Items => items;

        [field: GetBySelf]
        public Collider PackZone { get; private set; } = null!;

        [field: GetBySelf]
        public ExternalSize sizeInfo { get; private set; } = null!;

        public bool IsEmpty => items.IsEmpty();
        public bool HasEmptySlots => emptySlots.IsNotEmpty();

        public LayerMask SpawnObstaclesMask => spawnObstaclesMask.Deserialized ?? Physics.AllLayers;

        public Vector3 ItemRotation {
            get => itemRotation;
            set => itemRotation = value;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CCDisposable.Dispose(ref onPushedItem);
            CCDisposable.Dispose(ref onPoppedItem);
            CCDisposable.Dispose(ref onSpawnedItem);
        }

#if UNITY_EDITOR
        [Header("Debug")]
        [Space(6f)]

        [SerializeField]
        private bool spawnPointDebugEnabled;

        [SerializeField]
        private bool slotsDebugEnabled;

        private void OnDrawGizmos()
        {
            if (items.IsEmpty())
                return;

            var item = items.First().Value;

            if (spawnPointDebugEnabled)
                DrawSpawnPoints(item);

            if (slotsDebugEnabled)
                DrawGhostMeshes(item);
        }

        private void DrawSpawnPoints(IBoxItem item)
        {
            Gizmos.color = Color.green;

            var radius = Mathf.Max(item.sizeInfo.localBounds.size.magnitude / 2f, 0.2f);

            foreach (var pos in FindEmptyPositions(item))
                Gizmos.DrawSphere(pos, radius);
        }

        private void DrawGhostMeshes(IBoxItem item)
        {
            Gizmos.color = Color.yellow;

            var itemSize = itemSlotExtents!.Value * 2f;
            var itemBounds = item.sizeInfo.localBounds;
            itemBounds.size = itemSize;

            var packer = new BoundsPacker(PackZone.bounds, itemBounds)
            {
                Axis2 = Axis.Y,
                Axis3 = Axis.Z,
                IsAutoPaddingToFit = true
            };

            foreach (var slot in packer.Pack())
                Gizmos.DrawCube(cTransform.rotation * slot.center, slot.size);
        }
#endif

        public void Clear()
        {
            foreach (var item in items)
                RestoreItemState(item);
        }

        public bool TryPushItem(IBoxItem item)
        {
            CC.Guard.IsNotNull(item, nameof(item));

            if (!CanPushItem(item))
                return false;

            if (itemSlotExtents == null
                ||
                item.sizeInfo.localBounds.extents.NotNearlyEquals(itemSlotExtents.Value, 0.01f))
            {
                ResolveSlots(item);
            }

            if (emptySlots.Count == 0)
                return false;

            var slot = emptySlots.DeleteMin();
            var boxedItem = new BoxedItem(this, item, slot);

            PushItem(boxedItem);

            onPushedItem?.Execute(boxedItem);
            CCEventBus.Publish(new ItemBoxBoxedEvent(this, item));

            boxedItem.Setup();

            return true;
        }

        public bool TrySpawnItem(
            [NotNullWhen(true)] out IBoxItem? item,
            in Vector3? spawnRelativeToPoint = default
            )
        {
            item = default;

            var boxedItem = items.LastOrDefault();

            if (boxedItem == default)
                return false;

            if (!TrySpawnItemCore(boxedItem, spawnRelativeToPoint))
                return false;

            item = boxedItem.Value;
            RemoveItem(boxedItem);

            onSpawnedItem?.Execute(item);
            CCEventBus.Publish(new ItemBoxUnboxedEvent(this, item));
            return true;
        }

        public bool TryPopItem([NotNullWhen(true)] out IBoxItem? item)
        {
            if (!items.RemoveLast(out var boxedItem))
            {
                item = default;
                return false;
            }

            item = boxedItem.Value;
            RestoreItemState(boxedItem);

            var itemPos = item.transform.position;
            boxedItem.MoveTo(itemPos.AddY(item.sizeInfo.localBounds.size.y * 0.1f));

            RemoveItem(boxedItem);

            onPoppedItem?.Execute(item);
            CCEventBus.Publish(new ItemBoxUnboxedEvent(this, item));

            boxedItem.Restore();
            boxedItem.Dispose();
            return true;
        }

        public bool CanPushItem(IBoxItem? item)
        {
            if (item.IsNull())
                return false;

            var itemBounds = item.sizeInfo.localBounds;

            return itemBounds.extents.sqrMagnitude > 0f
                   &&
                   !pushedItems.Contains(item)
                   &&
                   (itemSlotExtents == null
                   ||
                   items.IsEmpty()
                   ||
                   itemBounds.extents.NearlyEquals(itemSlotExtents.Value, 0.01f)
                   );
        }

        public bool ContainsItem(IBoxItem? item)
        {
            if (item.IsNull())
                return false;

            return pushedItems.Contains(item);
        }

        public bool ContainsItem(Transform? transform)
        {
            if (transform == null)
                return false;

            for (int i = 0; i < items.Count; i++)
                if (items[i].Value.transform == transform)
                    return true;

            return false;
        }

        public bool ContainsItem(Collider? collider)
        {
            if (collider == null)
                return false;

            return ContainsItem(collider.transform);
        }

        public bool ContainsItem(Rigidbody? rigidbody)
        {
            if (rigidbody == null)
                return false;

            return ContainsItem(rigidbody.transform);
        }

        public Observable<BoxedItem> ObservePushItem()
        {
            onPushedItem ??= new ReactiveCommand<BoxedItem>();
            return onPushedItem;
        }

        public Observable<IBoxItem> ObserveSpawnItem()
        {
            onSpawnedItem ??= new ReactiveCommand<IBoxItem>();
            return onSpawnedItem;
        }

        public Observable<IBoxItem> ObservePopItem()
        {
            onPoppedItem ??= new ReactiveCommand<IBoxItem>();
            return onPoppedItem;
        }

        public IEnumerator<BoxedItem> GetEnumerator() => Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void PushItem(BoxedItem boxedItem)
        {
            items.Add(boxedItem);
            pushedItems.Add(boxedItem.Value);
        }

        private void MoveItemToSlot(
            BoxedItem boxedItem,
            Vector3 slot
            )
        {
            var worldSlot = cTransform.TransformPoint(slot);

            boxedItem.MoveTo(worldSlot);
        }

        private bool TrySpawnItemCore(
            in BoxedItem boxedItem,
            in Vector3? relativeToPoint
            )
        {
            var emptyPositions = FindEmptyPositions(boxedItem.Value);

            if (emptyPositions.IsEmpty())
                return false;

            Vector3 toSpawnPos;

            if (relativeToPoint != null)
                toSpawnPos = GetClosestEmptyPositionTo(relativeToPoint.Value, emptyPositions);
            else
                toSpawnPos = emptyPositions[0];

            boxedItem.MoveTo(toSpawnPos);
            return true;
        }

        private IReadOnlyList<Vector3> FindEmptyPositions(IBoxItem item)
        {
            var itemBounds = item.sizeInfo.bounds;
            var itemRadius = itemBounds.size.magnitude / 2f;
            var boxBounds = sizeInfo.bounds;
            var boxCenter = boxBounds.center;

            var boundsPoints = BoundsHelper.GetBoundsPoints(
                boxBounds.GetLocal(),
                boxBounds.extents
                );

            Vector3 worldPoint;
            Vector3 localPoint;

            var results = new List<Vector3>(boundsPoints.Count);

            var excludeX = spawnPointSearchExcludeAxes.HasFlagT(Axes.X);
            var excludeY = spawnPointSearchExcludeAxes.HasFlagT(Axes.Y);
            var excludeZ = spawnPointSearchExcludeAxes.HasFlagT(Axes.Z);

            for (int i = 0; i < spawnPointSeacrhSteps; i++)
            {
                for (int j = 0; j < boundsPoints.Count; j++)
                {
                    localPoint = boundsPoints[j];

                    if (excludeX && localPoint.x != boxCenter.x)
                        continue;

                    if (excludeY && localPoint.y != boxCenter.y)
                        continue;

                    if (excludeZ && localPoint.z != boxCenter.z)
                        continue;

                    localPoint = localPoint.DivideBy(transform.lossyScale); //Unscale
                    worldPoint = cTransform.TransformPoint(localPoint);

                    if (!Physics.CheckSphere(
                        worldPoint,
                        itemRadius,
                        SpawnObstaclesMask,
                        QueryTriggerInteraction.Ignore)
                        )
                    {
                        results.Add(worldPoint);
                    }
                }

                if (results.Count > 0)
                    break;
            }

            return results;
        }

        private void RemoveItem(in BoxedItem boxedItem)
        {
            items.Remove(boxedItem);
            pushedItems.Remove(boxedItem.Value);
        }

        private void ResolveSlots(IBoxItem item)
        {
            var boundsPacker = new BoundsPacker(sizeInfo.bounds, item.sizeInfo.bounds)
            {
                Axis2 = Axis.Y,
                Axis3 = Axis.Z,
                IsAutoPaddingToFit = true,
            };

            boundsPacker.PackNonAlloc(resolvedSlotsBuffer);

            for (int i = 0; i < emptySlots.Count; i++)
                emptySlots.DeleteMin();

            Vector3 slot;
            Bounds itemBounds = item.sizeInfo.bounds;
            Vector3 itemBoundsOffset = itemBounds.center - item.transform.position;

            for (int i = 0; i < resolvedSlotsBuffer.Count; i++)
            {
                slot = resolvedSlotsBuffer[i].center;
                //slot -= itemBoundsOffset;
                slot = cTransform.InverseTransformPoint(slot);

                emptySlots.Add(slot);
            }

            itemSlotExtents = itemBounds.extents;
            resolvedSlotsBuffer.Clear();
        }

        private void RestoreItemState(in BoxedItem boxedItem)
        {
            boxedItem.Value.transform.SetParent(null);

            emptySlots.Add(boxedItem.Slot);

            boxedItem.Restore();
        }

        private Vector3 GetClosestEmptyPositionTo(in Vector3 desiredPos, IReadOnlyList<Vector3> positions)
        {
            if (positions.IsEmpty())
                return Vector3.zero;

            Vector3? closestPoint = null;
            Vector3 toDesiredPosDir;
            Vector3? previousToDesiredPosDir = null;

            Vector3 point;

            for (int i = 0; i < positions.Count; i++)
            {
                point = positions[i];

                toDesiredPosDir = desiredPos - point;

                if (previousToDesiredPosDir == null
                    ||
                    toDesiredPosDir.sqrMagnitude < previousToDesiredPosDir.Value.sqrMagnitude)
                {
                    closestPoint = point;
                }

                previousToDesiredPosDir = toDesiredPosDir;
            }

            return closestPoint ?? positions[0];
        }
    }
}