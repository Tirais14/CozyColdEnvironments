using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    public readonly partial struct BoundsPacker : IEquatable<BoundsPacker>
    {
        public Bounds Container { get; private init; }
        public Bounds Item { get; private init; }

        public Axis Axis1 { get; private init; }
        public Axis? Axis2 { get; init; }
        public Axis? Axis3 { get; init; }

        public Vector3 Padding { get; init; }

        public bool IsAutoPaddingToFit { get; init; }

        public BoundsPacker Local {
            get
            {
                return new BoundsPacker
                {
                    Axis1 = Axis1,
                    Axis2 = Axis2,
                    Axis3 = Axis3,
                    Padding = Padding,
                    IsAutoPaddingToFit = IsAutoPaddingToFit
                };
            }
        }

        public BoundsPacker(
            Bounds container,
            Bounds item,
            Axis axis1 = Axis.X
            )
            :
            this()
        {
            Guard.IsNotDefault(container);
            Guard.IsNotDefault(item);

            Container = container;
            Item = item;
            Axis1 = axis1;
        }

        public static bool operator ==(BoundsPacker left, BoundsPacker right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BoundsPacker left, BoundsPacker right)
        {
            return !(left == right);
        }

        public static float GetPackedPadding(
            Bounds container,
            Bounds item,
            Axis axis
            )
        {
            var axisPointer = (int)axis;

            var itemSize = item.size[axisPointer];
            var itemCount = GetPackedCount(container, item, axis);

            return GetPackedPadding(itemCount, itemSize);
        }

        public static float GetPackedPadding(float itemCount, float itemSize)
        {
            Guard.IsGreaterThan(itemSize, 0);

            if (itemCount < 2f)
                return 0f;

            var itemCountFloored = Mathf.Floor(itemCount);
            var remainder = itemCount - itemCountFloored;

            var padding = remainder * itemSize / (itemCountFloored - 1);
            return padding;
        }

        public static float GetPackedCount(Bounds container, Bounds item, Axis axis)
        {
            if (container.extents.sqrMagnitude == 0)
                throw new ArgumentException("Container cannot be with zero size");
            if (item.extents.sqrMagnitude == 0)
                throw new ArgumentException("Item cannot be with zero size");

            var axisPointer = (int)axis;

            var cntSize = container.size[axisPointer];
            var itemSize = item.size[axisPointer];

            var itemCount = cntSize / itemSize;

            return itemCount;
        }

        #region Setters
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BoundsPacker WithMargin(Vector3 value)
        {
            if (Padding == value)
                return this;

            return new BoundsPacker()
            {
                Container = Container,
                Item = Item,
                Axis1 = Axis1,
                Axis2 = Axis2,
                Axis3 = Axis3,
                Padding = value,
                IsAutoPaddingToFit = IsAutoPaddingToFit
            };
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BoundsPacker WithContainer(Bounds value)
        {
            if (Container == value)
                return this;

            return new BoundsPacker()
            {
                Container = value,
                Item = Item,
                Axis1 = Axis1,
                Axis2 = Axis2,
                Axis3 = Axis3,
                Padding = Padding,
                IsAutoPaddingToFit = IsAutoPaddingToFit
            };
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BoundsPacker WithItem(Bounds value)
        {
            if (Item == value)
                return this;

            return new BoundsPacker()
            {
                Container = Container,
                Item = value,
                Axis1 = Axis1,
                Axis2 = Axis2,
                Axis3 = Axis3,
                Padding = Padding,
                IsAutoPaddingToFit = IsAutoPaddingToFit
            };
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BoundsPacker WithAxis1(Axis value)
        {
            if (Axis1 == value)
                return this;

            return new BoundsPacker()
            {
                Container = Container,
                Item = Item,
                Axis1 = value,
                Axis2 = Axis2,
                Axis3 = Axis3,
                Padding = Padding,
                IsAutoPaddingToFit = IsAutoPaddingToFit
            };
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BoundsPacker WithAxis2(Axis? value)
        {
            if (Axis2 == value)
                return this;

            return new BoundsPacker()
            {
                Container = Container,
                Item = Item,
                Axis1 = Axis1,
                Axis2 = value,
                Axis3 = Axis3,
                Padding = Padding,
                IsAutoPaddingToFit = IsAutoPaddingToFit
            };
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BoundsPacker WithAxis3(Axis? value)
        {
            if (Axis3 == value)
                return this;

            return new BoundsPacker()
            {
                Container = Container,
                Item = Item,
                Axis1 = Axis1,
                Axis2 = Axis2,
                Axis3 = value,
                Padding = Padding,
                IsAutoPaddingToFit = IsAutoPaddingToFit
            };
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BoundsPacker WithMarginToFit(bool value)
        {
            if (IsAutoPaddingToFit == value)
                return this;

            return new BoundsPacker()
            {
                Container = Container,
                Item = Item,
                Axis1 = Axis1,
                Axis2 = Axis2,
                Axis3 = Axis3,
                Padding = Padding,
                IsAutoPaddingToFit = value
            };
        }
        #endregion Setters


        public readonly IReadOnlyList<Bounds> Pack()
        {
            int axisCount = (Axis2 == null ? 0 : 1) + (Axis3 == null ? 0 : 1) + 1;

            return axisCount switch
            {
                1 => PackByAxis(null),
                2 => PackByTwoAxes(null),
                3 => PackByThreeAxes(null),
                _ => throw new InvalidOperationException(axisCount.ToString()),
            };
        }
        public readonly void PackNonAlloc(List<Bounds> results)
        {
            int axisCount = (Axis2 == null ? 0 : 1) + (Axis3 == null ? 0 : 1) + 1;
            results.Clear();

            _ = axisCount switch
            {
                1 => PackByAxis(results),
                2 => PackByTwoAxes(results),
                3 => PackByThreeAxes(results),
                _ => throw new InvalidOperationException(axisCount.ToString()),
            };
        }

        private readonly IReadOnlyList<Bounds> PackByAxis(List<Bounds>? results)
        {
            var axisPointer = (int)Axis1;

            var itemSize = Item.size;

            if (Padding[axisPointer] != 0f)
                itemSize[axisPointer] += itemSize[axisPointer] * Mathf.Max(Padding[axisPointer], 0f);

            var itemCenter = Container.min + Item.extents;

            var testItem = Item;
            var testItemCenter = itemCenter;
            testItem.center = testItemCenter;

            var packedCount = GetPackedCount(Container, testItem, Axis1);

            var gap = GetPackedPadding(packedCount, itemSize[axisPointer]);

            var loopFuse = LoopFuse.Create(iterationLimit: int.MaxValue / 10);

            results ??= new List<Bounds>(Mathf.FloorToInt(packedCount));

            while (Container.Contains(testItem)
                   &&
                   loopFuse.MoveNext())
            {
                results.Add(testItem);

                testItemCenter[axisPointer] += itemSize[axisPointer] + gap;

                testItem.center = testItemCenter;
            }

            return results;
        }

        private readonly IReadOnlyList<Bounds> PackByTwoAxes(List<Bounds>? results)
        {
            var resolvedAxis2 = GetSecondAxis();

            var axis1 = (int)Axis1;
            var axis2 = (int)resolvedAxis2;

            var item = Item;

            if (Padding != Vector3.zero)
            {
                var itemSize = Item.size;

                itemSize[axis1] += itemSize[axis1] * Mathf.Max(Padding[axis1], 0f);
                itemSize[axis2] += itemSize[axis2] * Mathf.Max(Padding[axis2], 0f);

                item.size = itemSize;
            }

            var gap = Vector3.zero;

            var axis1ItemCount = GetPackedCount(Container, item, Axis1);
            var axis2ItemCount = GetPackedCount(Container, item, resolvedAxis2);

            if (IsAutoPaddingToFit)
            {
                gap[axis1] = GetPackedPadding(axis1ItemCount, item.size[axis1]);
                gap[axis2] = GetPackedPadding(axis2ItemCount, item.size[axis2]);
            }

            var startCenter = Container.min + item.extents;

            Vector3 nextCenter;
            Bounds nextItem;

            var loopFuse = LoopFuse.Create(iterationLimit: int.MaxValue / 10);

            float nextAxis1Pos;
            float nextAxis2Pos;

            var axis1ItemCountFloored = Mathf.FloorToInt(axis1ItemCount);
            var axis2ItemCountFloored = Mathf.FloorToInt(axis2ItemCount);

            results ??= new List<Bounds>(
                axis1ItemCountFloored 
                * 
                axis2ItemCountFloored
                );

            for (int i2 = 0; i2 < axis2ItemCountFloored; i2++)
            {
                nextAxis2Pos = i2 * (item.size[axis2] + gap[axis2]);

                for (int i1 = 0; i1 < axis1ItemCountFloored ; i1++)
                {
                    if (!loopFuse.MoveNext())
                        return results;

                    nextAxis1Pos = i1 * (item.size[axis1] + gap[axis1]);

                    nextCenter = startCenter;
                    nextCenter[axis1] += nextAxis1Pos;
                    nextCenter[axis2] += nextAxis2Pos;

                    nextItem = item;
                    nextItem.center = nextCenter;

                    results.Add(nextItem);
                }
            }

            return results;
        }

        private Axis GetSecondAxis()
        {
            if (Axis2 == null)
            {
                if (Axis3 == null)
                    throw new InvalidOperationException("Not found second axis");

                return Axis3.Value;
            }

            return Axis2.Value;
        }

        private readonly IReadOnlyList<Bounds> PackByThreeAxes(List<Bounds>? results)
        {
            if (Axis2 == null)
                throw new InvalidOperationException("Missing second axis");
            if (Axis3 == null)
                throw new InvalidOperationException("Missing third axis");

            var item = Item;

            var axis1 = (int)Axis1;
            var axis2 = (int)Axis2;
            var axis3 = (int)Axis3;

            if (Padding != Vector3.zero)
            {
                var itemSize = item.size;

                itemSize[axis1] += itemSize[axis1] * Padding[axis1];
                itemSize[axis2] += itemSize[axis2] * Padding[axis2];
                itemSize[axis3] += itemSize[axis3] * Padding[axis3];

                item.size = itemSize;
            }

            var gap = Vector3.zero;

            var axis1ItemCount = GetPackedCount(Container, item, Axis1);
            var axis2ItemCount = GetPackedCount(Container, item, Axis2.Value);
            var axis3ItemCount = GetPackedCount(Container, item, Axis3.Value);

            if (IsAutoPaddingToFit)
            {
                gap[axis1] = GetPackedPadding(axis1ItemCount, item.size[axis1]);
                gap[axis2] = GetPackedPadding(axis2ItemCount, item.size[axis2]);
                gap[axis3] = GetPackedPadding(axis3ItemCount, item.size[axis3]);
            }

            var startCenter = Container.min + item.extents;

            var loopFuse = LoopFuse.Create(iterationLimit: int.MaxValue / 10);

            float nextAxis1Pos;
            float nextAxis2Pos;
            float nextAxis3Pos;

            Vector3 nextCenter;
            Bounds nextItem;

            var axis1ItemCountFloored = Mathf.FloorToInt(axis1ItemCount);
            var axis2ItemCountFloored = Mathf.FloorToInt(axis2ItemCount);
            var axis3ItemCountFloored = Mathf.FloorToInt(axis3ItemCount);

            results ??= new List<Bounds>(
                Mathf.FloorToInt(axis1ItemCount)
                *
                Mathf.FloorToInt(axis2ItemCount)
                *
                Mathf.FloorToInt(axis3ItemCount)
                );


            for (int i2 = 0; i2 < axis2ItemCountFloored; i2++)
            {
                nextAxis2Pos = i2 * (item.size[axis2] + gap[axis2]);

                for (int i3 = 0; i3 < axis3ItemCountFloored; i3++)
                {
                    nextAxis3Pos = i3 * (item.size[axis3] + gap[axis3]);

                    for (int i1 = 0; i1 < axis1ItemCountFloored; i1++)
                    {
                        if (!loopFuse.MoveNext())
                            return results;
                        nextAxis1Pos = i1 * (item.size[axis1] + gap[axis1]);

                        nextCenter = startCenter;
                        nextCenter[axis1] += nextAxis1Pos;
                        nextCenter[axis2] += nextAxis2Pos;
                        nextCenter[axis3] += nextAxis3Pos;

                        nextItem = item;
                        nextItem.center = nextCenter;

                        results.Add(nextItem);
                    }
                }
            }

            return results;
        }

        public readonly override bool Equals(object? obj)
        {
            return obj is BoundsPacker packer && Equals(packer);
        }

        public readonly bool Equals(BoundsPacker other)
        {
            return Container.Equals(other.Container)
                   &&
                   Item.Equals(other.Item)
                   &&
                   Axis1 == other.Axis1
                   &&
                   Axis2 == other.Axis2
                   &&
                   Axis3 == other.Axis3
                   &&
                   Padding.Equals(other.Padding)
                   &&
                   IsAutoPaddingToFit == other.IsAutoPaddingToFit
                   &&
                   EqualityComparer<BoundsPacker>.Default.Equals(Local, other.Local);
        }

        public readonly override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Container);
            hash.Add(Item);
            hash.Add(Axis1);
            hash.Add(Axis2);
            hash.Add(Axis3);
            hash.Add(Padding);
            hash.Add(IsAutoPaddingToFit);
            hash.Add(Local);
            return hash.ToHashCode();
        }
    }
}
