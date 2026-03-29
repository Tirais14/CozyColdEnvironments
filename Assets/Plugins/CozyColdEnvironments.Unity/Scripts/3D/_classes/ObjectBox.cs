using CCEnvs;
using CCEnvs.Collections;
using CCEnvs.Unity;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.EditorSerialization;
using CCEnvs.Unity.Injections;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
#nullable enable
namespace CCEnvs.Unity.D3
{
    [RequireComponent(typeof(Collider))]
    public class ObjectBox<T> : CCBehaviour, IEnumerable<BoxedItem<T>>
        where T : IBoxItem
    {
        public const int EMPTY_POSITIONS_SEARCH_STEP_COUNT_MIN = 1;

        private readonly List<BoxedItem<T>> items = new();

        private readonly Queue<Vector3> emptySlots = new();

        private readonly HashSet<T> pushedItems = new();

        [Header("Item Settings")]
        [Space(6f)]

        [SerializeField]
        private bool collisionEnabled;

        [Header("Pack Settings")]
        [Space(6f)]

        [SerializeField]
        private Vector3 packMarginPercentage = new(0.1f, 0f, 0.1f);

        [Header("Spawn Settings")]
        [Space(6f)]

        [SerializeField]
        private SerializedNullable<LayerMask> spawnObstaclesMask;

        [SerializeField, Min(EMPTY_POSITIONS_SEARCH_STEP_COUNT_MIN)]
        private int emptyPositionsSearchStepCount = 1;

        [SerializeField]
        private Axes emptyPositionsSearchExcludeAxes = Axes.None;

        private Bounds? exampleBounds;

        [field: GetBySelf]
        public Collider packZone { get; private set; } = null!;

        public IReadOnlyCollection<BoxedItem<T>> Items => items;

        public bool IsEmpty => items.IsEmpty();
        public bool HasEmptySlots => emptySlots.IsNotEmpty();

        public LayerMask SpawnObstaclesMask => spawnObstaclesMask.Deserialized ?? Physics.AllLayers;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (items.IsEmpty())
                return;

            Gizmos.color = Color.red;

            var item = items.First().Value;
            var radius = Mathf.Max(item.bounds.size.magnitude / 2f, 0.2f);

            foreach (var pos in FindEmptyPositions(item))
                Gizmos.DrawSphere(pos, radius);
        }
#endif

        public void Clear()
        {
            foreach (var item in items)
                RestoreItemState(item);
        }

        public bool TryPushItem(T item)
        {
            CC.Guard.IsNotNull(item, nameof(item));

            if (!CanPushItem(item))
                return false;

            if (item.bounds != exampleBounds)
                ResolveSlots(item);

            if (!emptySlots.TryDequeue(out var slot))
                return false;

            PushItem(item, slot);

            item.transform.SetParent(cTransform);

            return true;
        }

        public bool TryPopItem(
            [NotNullWhen(true)] out T? item,
            Vector3? spawnRelativeToPoint
            )
        {
            if (items.Count == 0
                ||
                !items.RemoveLast(out var boxedItem))
            {
                item = default;
                return false;
            }

            RestoreItemState(boxedItem);
            SpawnItem(boxedItem, spawnRelativeToPoint);
            pushedItems.Remove(boxedItem.Value);

            item = boxedItem.Value;
            boxedItem.Dispose();
            return true;
        }

        public bool CanPushItem(T? item)
        {
            return item.IsNotNull()
                   &&
                   item.bounds.size.sqrMagnitude > 0f
                   &&
                   (exampleBounds == null
                   ||
                   items.IsEmpty()
                   ||
                   item.bounds.GetLocal() == exampleBounds
                   );
        }

        public IEnumerator<BoxedItem<T>> GetEnumerator() => Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void ResolveSlots(T item)
        {
            var itemSize = item.collider.bounds.size;
            var margin = itemSize.MultiplyBy(packMarginPercentage);

            var boundsPacker = new BoundsPacker(packZone.bounds, item.collider.bounds)
            {
                Axis2 = Axis.Y,
                Axis3 = Axis.Z,
                Margin = margin,
                CacheResults = true,
                MarginToFit = true,
            };

            var positions = boundsPacker.Pack();

            emptySlots.Clear();

            for (int i = 0; i < positions.Count; i++)
                emptySlots.Enqueue(positions[i].center);

            exampleBounds = item.bounds;
        }

        private BoxedItem<T> PushItem(T item, Vector3 slot)
        {
            var entry = new BoxedItem<T>(item, slot);
            entry.Setup();

            items.Add(entry);

            return entry;
        }

        private void SpawnItem(
            BoxedItem<T> boxedItem,
            Vector3? relativeToPoint
            )
        {
            var emptyPositions = FindEmptyPositions(boxedItem.Value);

            if (emptyPositions.IsEmpty())
                return;

            //TOOD: Realize relative spawn
            boxedItem.MoveTo(emptyPositions[0]);
        }

        private void RestoreItemState(BoxedItem<T> boxedItem)
        {
            boxedItem.Value.transform.SetParent(null);

            emptySlots.Enqueue(boxedItem.Slot);

            boxedItem.Restore();
        }

        private Vector3 GetClosestEmptyPositionTo(Vector3 desiredPos, IReadOnlyList<Vector3> positions)
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

        private IReadOnlyList<Vector3> FindEmptyPositions(IBoxItem item)
        {
            var itemBounds = item.bounds;
            var itemRadius = itemBounds.size.magnitude / 2f;
            var boxBounds = packZone.bounds;
            var boxCenter = boxBounds.center;
             
            var boundsPoints = BoundsHelper.GetBoundsPoints(
                boxBounds.GetLocal(),
                boxBounds.extents,
                cache: true
                );

            Vector3 worldPoint;
            Vector3 localPoint;

            var results = new List<Vector3>(boundsPoints.Count);

            var excludeX = emptyPositionsSearchExcludeAxes.IsFlagSetted(Axes.X);
            var excludeY = emptyPositionsSearchExcludeAxes.IsFlagSetted(Axes.Y);
            var excludeZ = emptyPositionsSearchExcludeAxes.IsFlagSetted(Axes.Z);

            for (int i = 0; i < emptyPositionsSearchStepCount; i++)
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

                if (results.Count != 0)
                    break;
            }

            return results;
        }
    }
}