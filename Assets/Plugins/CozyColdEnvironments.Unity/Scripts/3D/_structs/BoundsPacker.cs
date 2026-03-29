using CommunityToolkit.Diagnostics;
using Generator.Equals;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    [Equatable]
    public readonly partial struct BoundsPacker
    {
        private readonly static Lazy<ConcurrentDictionary<BoundsPacker, IReadOnlyList<Bounds>>> cache = new(() => new());

        public Bounds Container { get; private init; }
        public Bounds Item { get; private init; }

        public Axis Axis1 { get; private init; }
        public Axis? Axis2 { get; init; }
        public Axis? Axis3 { get; init; }

        public Vector3 Margin { get; init; }

        public bool MarginToFit { get; init; }
        public bool CacheResults { get; init; }

        public BoundsPacker Local {
            get
            {
                return new BoundsPacker
                {
                    Container = containerLocal,
                    Item = itemLocal,
                    Axis1 = Axis1,
                    Axis2 = Axis2,
                    Axis3 = Axis3,
                    Margin = Margin,
                    MarginToFit = MarginToFit,
                    CacheResults = CacheResults
                };
            }
        }

        private Bounds containerLocal {
            get
            {
                var container = Container;
                container.center = Vector3.zero;

                return container;
            }
        }

        private Bounds itemLocal {
            get
            {
                var item = Item;
                item.center = Vector3.zero;

                return item;
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

        #region Setters
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BoundsPacker WithMargin(Vector3 value)
        {
            if (Margin == value)
                return this;

            return new BoundsPacker()
            {
                Container = Container,
                Item = Item,
                Axis1 = Axis1,
                Axis2 = Axis2,
                Axis3 = Axis3,
                Margin = value,
                MarginToFit = MarginToFit,
                CacheResults = CacheResults
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
                Margin = Margin,
                MarginToFit = MarginToFit,
                CacheResults = CacheResults
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
                Margin = Margin,
                MarginToFit = MarginToFit,
                CacheResults = CacheResults
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
                Margin = Margin,
                MarginToFit = MarginToFit,
                CacheResults = CacheResults
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
                Margin = Margin,
                MarginToFit = MarginToFit,
                CacheResults = CacheResults
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
                Margin = Margin,
                MarginToFit = MarginToFit,
                CacheResults = CacheResults
            };
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BoundsPacker WithMarginToFit(bool value)
        {
            if (MarginToFit == value)
                return this;

            return new BoundsPacker()
            {
                Container = Container,
                Item = Item,
                Axis1 = Axis1,
                Axis2 = Axis2,
                Axis3 = Axis3,
                Margin = Margin,
                MarginToFit = value,
                CacheResults = CacheResults
            };
        }


        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BoundsPacker WithCacheCalculations(bool value)
        {
            if (CacheResults == value)
                return this;

            return new BoundsPacker()
            {
                Container = Container,
                Item = Item,
                Axis1 = Axis1,
                Axis2 = Axis2,
                Axis3 = Axis3,
                Margin = Margin,
                MarginToFit = MarginToFit,
                CacheResults = value
            };
        }
        #endregion Setters

        public readonly IReadOnlyList<Bounds> Pack()
        {
            int axisCount = (Axis2 == null ? 0 : 1) + (Axis3 == null ? 0 : 1) + 1;

            switch (axisCount)
            {
                case 1:
                    {
                        return PackByAxis();
                    }
                case 2:
                    {
                        Axis secondAxis;

                        if (Axis2 == null)
                            secondAxis = Axis3!.Value;
                        else
                            secondAxis = Axis2.Value;

                        return PackByTwoAxes();
                    }
                case 3:
                    {
                        return PackByThreeAxes();
                    }
                default:
                    throw new InvalidOperationException();
            }
        }

        private readonly IReadOnlyList<Bounds> PackByAxis()
        {
            var local = Local;

            if (TryGetCachedItems(local, out var cachedResults, !Container.IsLocal()))
                return cachedResults;

            var axisPointer = (int)Axis1;

            var itemSize = Item.size;

            if (Margin[axisPointer] != 0f)
                itemSize[axisPointer] += itemSize[axisPointer] * Mathf.Max(Margin[axisPointer], 0f);

            var itemCenter = Container.min + Item.extents;

            var testItem = Item;
            var testItemCenter = itemCenter;
            testItem.center = testItemCenter;

            var loopFuse = LoopFuse.Create(iterationLimit: int.MaxValue / 100);

            var results = new List<Bounds>();

            while (Container.Contains(testItem)
                   &&
                   loopFuse.MoveNext())
            {
                results.Add(testItem);

                testItemCenter[axisPointer] += itemSize[axisPointer];

                testItem.center = testItemCenter;
            }

            TryCacheItems(local, results, !Container.IsLocal());
            return results;
        }

        private readonly IReadOnlyList<Bounds> PackByTwoAxes()
        {
            var local = Local;

            if (TryGetCachedItems(local, out var cachedResults, !Container.IsLocal()))
                return cachedResults;

            var axis1 = (int)Axis1;
            var axis2 = (int)GetSecondAxis();

            var item = Item;

            if (Margin != Vector3.zero)
            {
                var itemSize = Item.size;

                itemSize[axis1] += itemSize[axis1] * Mathf.Max(Margin[axis1], 0f);
                itemSize[axis2] += itemSize[axis2] * Mathf.Max(Margin[axis2], 0f);

                item.size = itemSize;
            }

            var startCenter = Container.min + item.extents;

            Vector3 testCenter;
            Bounds testItem;

            var loopFuse = LoopFuse.Create(iterationLimit: int.MaxValue / 100);

            float p1;
            float p2;

            var results = new List<Bounds>();

            for (int i2 = 0; ; i2++)
            {
                p2 = i2 * item.size[axis2];

                for (int i1 = 0; ; i1++)
                {
                    if (!loopFuse.MoveNext())
                    {
                        TryCacheItems(local, results, !Container.IsLocal());
                        return results;
                    }

                    p1 = i1 * item.size[axis1];

                    testCenter = startCenter;
                    testCenter[axis1] += p1 + Margin[axis1];
                    testCenter[axis2] += p2 + Margin[axis2];

                    testItem = item;
                    testItem.center = testCenter;

                    if (!Container.Contains(testItem))
                    {
                        if (i1 == 0)
                        {
                            TryCacheItems(local, results, !Container.IsLocal());
                            return results;
                        }

                        break;
                    }

                    results.Add(testItem);
                }
            }
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

        private readonly IReadOnlyList<Bounds> PackByThreeAxes()
        {
            if (Axis2 == null)
                throw new InvalidOperationException("Missing second axis");
            if (Axis3 == null)
                throw new InvalidOperationException("Missing third axis");

            var local = Local;

            if (TryGetCachedItems(local, out var cachedResults, !Container.IsLocal()))
                return cachedResults;

            var axis1 = (int)Axis1;
            var axis2 = (int)Axis2;
            var axis3 = (int)Axis2;

            var item = Item;

            if (Margin != Vector3.zero)
            {
                var itemSize = item.size;

                itemSize[axis1] += itemSize[axis1] * Mathf.Max(Margin[axis1], 0f);
                itemSize[axis2] += itemSize[axis2] * Mathf.Max(Margin[axis2], 0f);
                itemSize[axis3] += itemSize[axis3] * Mathf.Max(Margin[axis3], 0f);

                item.size = itemSize;
            }

            var startCenter = Container.min + item.extents;

            Vector3 testCenter;
            Bounds testItem;

            var loopFuse = LoopFuse.Create(iterationLimit: int.MaxValue / 100);

            float p1;
            float p2;
            float p3;

            var results = new List<Bounds>();

            bool breakI3 = false;

            for (int i2 = 0; ; i2++)
            {
                p2 = i2 * item.size[axis2];

                for (int i3 = 0; ; i3++)
                {
                    p3 = i3 * item.size[axis3];

                    for (int i1 = 0; ; i1++)
                    {
                        if (!loopFuse.MoveNext())
                        {
                            TryCacheItems(local, results, !Container.IsLocal());
                            return results;
                        }

                        p1 = i1 * item.size[axis1];

                        testCenter = startCenter;
                        testCenter[axis1] += p1 + Margin[axis1];
                        testCenter[axis2] += p2 + Margin[axis2];
                        testCenter[axis3] += p3 + Margin[axis3];

                        testItem = item;
                        testItem.center = testCenter;

                        if (!Container.Contains(testItem))
                        {
                            if (i1 != 0) break;
                            else if (i3 != 0) breakI3 = true;
                            else
                            {
                                TryCacheItems(local, results, !Container.IsLocal());
                                return results;
                            }

                            break;
                        }

                        results.Add(testItem);
                    }

                    if (breakI3)
                    {
                        breakI3 = false;
                        break;
                    }
                }
            }
        }

        private readonly bool TryGetCachedItems(
            BoundsPacker local,
            [NotNullWhen(true)] out IReadOnlyList<Bounds>? results,
            bool mustTransformed
            )
        {
            if (cache.IsValueCreated
                &&
                cache.Value.TryGetValue(local, out var positions))
            {
                if (mustTransformed)
                    results = BoundsHelper.TransformBounds(Container, positions);
                else
                    results = positions;

                return true;
            }

            results = null;
            return false;
        }

        private readonly void TryCacheItems(
            BoundsPacker local,
            IReadOnlyList<Bounds> items,
            bool mustTransformed
            )
        {
            if (!CacheResults
                ||
                (cache.IsValueCreated
                &&
                cache.Value.ContainsKey(local)))
            {
                return;
            }

            Transform t;

            if (mustTransformed)
                items = BoundsHelper.InverseTransformBounds(Container, items);

            cache.Value[local] = items;
        }
    }
}
