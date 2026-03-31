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
    [RequireComponent(typeof(Collider))]
    public class ItemBox: CCBehaviour, IEnumerable<BoxedItem>
    {
        public const int EMPTY_POSITION_SEARCH_STEP_COUNT_MIN = 1;

        private readonly List<BoxedItem> _items = new();

        private readonly C5.IntervalHeap<Vector3> _emptySlots = new(Comparer<Vector3>.Create((left, right) => left.y.CompareTo(right.y)));

        private readonly HashSet<IBoxItem> _pushedItems = new();

        [Header("Spawn Settings")]
        [Space(6f)]

        [SerializeField]
        private SerializedNullable<LayerMask> _spawnObstaclesMask;

        [SerializeField, Min(EMPTY_POSITION_SEARCH_STEP_COUNT_MIN)]
        private int _emptyPositionSearchStepCount = 1;

        [SerializeField]
        private Axes _emptyPositionSearchExcludeAxes = Axes.None;

        private Vector3? _itemSlotExtents;

        private ReactiveCommand<BoxedItem>? _onPushedItem;
        private ReactiveCommand<IBoxItem>? _onSpawnedItem;
        private ReactiveCommand<IBoxItem>? _onPoppedItem;

        [field: GetBySelf]
        public Collider packZone { get; private set; } = null!;

        public IReadOnlyCollection<BoxedItem> Items => _items;

        public bool IsEmpty => _items.IsEmpty();
        public bool HasEmptySlots => _emptySlots.IsNotEmpty();

        public LayerMask SpawnObstaclesMask => _spawnObstaclesMask.Deserialized ?? Physics.AllLayers;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CCDisposable.Dispose(ref _onPushedItem);
            CCDisposable.Dispose(ref _onPoppedItem);
            CCDisposable.Dispose(ref _onSpawnedItem);
        }

#if UNITY_EDITOR
        [Header("Debug")]
        [Space(6f)]

        [SerializeField]
        private bool emptyPositionSearchDebugEnabled;

        [SerializeField]
        private bool slotsDebugEnabled;

        private void OnDrawGizmos()
        {
            if (_items.IsEmpty())
                return;

            var item = _items.First().Value;

            if (emptyPositionSearchDebugEnabled)
            {
                Gizmos.color = Color.green;

                var radius = Mathf.Max(item.GetBounds().size.magnitude / 2f, 0.2f);

                foreach (var pos in FindEmptyPositions(item))
                    Gizmos.DrawSphere(pos, radius);
            }

            if (slotsDebugEnabled)
            {
                Gizmos.color = Color.yellow;

                var itemSize = _itemSlotExtents!.Value * 2f;
                var itemBounds = item.GetBounds();
                itemBounds.size = itemSize;

                var packer = new BoundsPacker(packZone.bounds, itemBounds)
                {
                    Axis2 = Axis.Y,
                    Axis3 = Axis.Z,
                    IsAutoPaddingToFit = true
                };

                foreach (var slot in packer.Pack())
                    Gizmos.DrawCube(slot.center, slot.size);
            }
        }
#endif

        public void Clear()
        {
            foreach (var item in _items)
                RestoreItemState(item);
        }

        public bool TryPushItem(IBoxItem item)
        {
            CC.Guard.IsNotNull(item, nameof(item));

            if (!CanPushItem(item))
                return false;

            if (_itemSlotExtents == null
                ||
                item.GetBounds().extents.NotNearlyEquals(_itemSlotExtents.Value, 0.01f))
            {
                ResolveSlots(item);
            }

            if (_emptySlots.Count == 0)
                return false;

            var slot = _emptySlots.DeleteMin();
            var boxedItem = new BoxedItem(item, slot);

            PushItem(boxedItem);

            _onPushedItem?.Execute(boxedItem);
            CCEventBus.Publish(new ItemBoxBoxedEvent(this, item));

            boxedItem.Setup(cTransform);
            MoveItemToSlot(boxedItem, slot);
            return true;
        }

        public bool TrySpawnItem(
            [NotNullWhen(true)] out IBoxItem? item,
            in Vector3? spawnRelativeToPoint = default
            )
        {
            item = default;

            var boxedItem = _items.LastOrDefault();

            if (boxedItem == default)
                return false;

            if (!TrySpawnItemCore(boxedItem, spawnRelativeToPoint))
                return false;

            item = boxedItem.Value;
            RemoveItem(boxedItem);

            _onSpawnedItem?.Execute(item);
            CCEventBus.Publish(new ItemBoxUnboxedEvent(this, item));
            return true;
        }

        public bool TryPopItem([NotNullWhen(true)] out IBoxItem? item)
        {
            if (!_items.RemoveLast(out var boxedItem))
            {
                item = default;
                return false;
            }

            item = boxedItem.Value;
            RestoreItemState(boxedItem);

            var itemPos = item.transform.position;
            boxedItem.MoveTo(itemPos.AddY(item.GetBounds().size.y * 0.1f));

            RemoveItem(boxedItem);

            _onPoppedItem?.Execute(item);
            CCEventBus.Publish(new ItemBoxUnboxedEvent(this, item));

            boxedItem.Restore();
            boxedItem.Dispose();
            return true;
        }

        public bool CanPushItem(IBoxItem? item)
        {
            if (item.IsNull())
                return false;

            var itemBounds = item.GetBounds();

            return itemBounds.extents.sqrMagnitude > 0f
                   &&
                   !_pushedItems.Contains(item)
                   &&
                   (_itemSlotExtents == null
                   ||
                   _items.IsEmpty()
                   ||
                   itemBounds.extents.NearlyEquals(_itemSlotExtents.Value, 0.01f)
                   );
        }

        public bool ContainsItem(Transform transform)
        {
            for (int i = 0; i < _items.Count; i++)
                if (_items[i].Value.transform == transform)
                    return true;

            return false;
        }

        public bool ContainsItem(Collider collider)
        {
            return ContainsItem(collider.transform);
        }

        public bool ContainsItem(Rigidbody rigidbody)
        {
            return ContainsItem(rigidbody.transform);
        }

        public Observable<BoxedItem> ObservePushItem()
        {
            _onPushedItem ??= new ReactiveCommand<BoxedItem>();
            return _onPushedItem;
        }

        public Observable<IBoxItem> ObserveSpawnItem()
        {
            _onSpawnedItem ??= new ReactiveCommand<IBoxItem>();
            return _onSpawnedItem;
        }

        public Observable<IBoxItem> ObservePopItem()
        {
            _onPoppedItem ??= new ReactiveCommand<IBoxItem>();
            return _onPoppedItem;
        }

        public IEnumerator<BoxedItem> GetEnumerator() => Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void PushItem(BoxedItem boxedItem)
        {
            _items.Add(boxedItem);
            _pushedItems.Add(boxedItem.Value);
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
            var itemBounds = item.GetBounds();
            var itemRadius = itemBounds.size.magnitude / 2f;
            var boxBounds = packZone.bounds;
            var boxCenter = boxBounds.center;

            var boundsPoints = BoundsHelper.GetBoundsPoints(
                boxBounds.GetLocal(),
                boxBounds.extents
                );

            Vector3 worldPoint;
            Vector3 localPoint;

            var results = new List<Vector3>(boundsPoints.Count);

            var excludeX = _emptyPositionSearchExcludeAxes.HasFlagT(Axes.X);
            var excludeY = _emptyPositionSearchExcludeAxes.HasFlagT(Axes.Y);
            var excludeZ = _emptyPositionSearchExcludeAxes.HasFlagT(Axes.Z);

            for (int i = 0; i < _emptyPositionSearchStepCount; i++)
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
            _items.Remove(boxedItem);
            _pushedItems.Remove(boxedItem.Value);
        }

        private void ResolveSlots(IBoxItem item)
        {
            var packZoneBoundsLocal = packZone.bounds.GetLocal();

            var boundsPacker = new BoundsPacker(packZone.bounds, item.collider.bounds)
            {
                Axis2 = Axis.Y,
                Axis3 = Axis.Z,
                IsAutoPaddingToFit = true,
            };

            var positions = boundsPacker.Pack();

            for (int i = 0; i < _emptySlots.Count; i++)
                _emptySlots.DeleteMax();

            Vector3 slot;
            Bounds itemBounds = item.GetBounds();
            Vector3 itemBoundsOffset = itemBounds.center - item.transform.position;

            for (int i = 0; i < positions.Count; i++)
            {
                slot = positions[i].center;
                slot -= itemBoundsOffset;
                slot = cTransform.InverseTransformPoint(slot);

                _emptySlots.Add(slot);
            }

            _itemSlotExtents = itemBounds.extents;
        }

        private void RestoreItemState(in BoxedItem boxedItem)
        {
            boxedItem.Value.transform.SetParent(null);

            _emptySlots.Add(boxedItem.Slot);

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