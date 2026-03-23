using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    public static class BoundsPacker
    {
        //private static readonly Lazy<Dictionary<PackCacheKey, Bounds[]>> packCache = new(
        //    () => new Dictionary<PackCacheKey, Bounds[]>()
        //    );

        public static IReadOnlyList<Bounds> PackByAxis(
            Bounds container,
            Bounds item,
            Axis axis,
            float margin = 0f
            )
        {
            var axisPointer = (int)axis;

            var itemSize = item.size;

            if (margin != 0f)
                itemSize[axisPointer] += itemSize[axisPointer] * Mathf.Max(margin, 0f);

            var itemCenter = container.min + item.extents;

            var testItem = item;
            var testItemCenter = itemCenter;
            testItem.center = testItemCenter;

            var loopFuse = LoopFuse.Create(iterationLimit: int.MaxValue / 100);

            var results = new List<Bounds>();

            while (container.Contains(testItem)
                   &&
                   loopFuse.MoveNext())
            {
                results.Add(testItem);

                testItemCenter[axisPointer] += itemSize[axisPointer];

                testItem.center = testItemCenter;
            }

            return results;
        }

        public static IReadOnlyList<Bounds> PackByTwoAxes(
            Bounds container,
            Bounds item,
            Axis firstAxis,
            Axis secondAxis,
            Vector2 margin = default
            )
        {
            var axis1 = (int)firstAxis;
            var axis2 = (int)secondAxis;

            if (margin != Vector2.zero)
            {
                var itemSize = item.size;

                itemSize[axis1] += itemSize[axis1] * Mathf.Max(margin[axis1], 0f);
                itemSize[axis2] += itemSize[axis2] * Mathf.Max(margin[axis2], 0f);

                item.size = itemSize;
            }

            var startCenter = container.min + item.extents;

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
                        return results;

                    p1 = i1 * item.size[axis1];

                    testCenter = startCenter;
                    testCenter[axis1] += p1 + margin[axis1];
                    testCenter[axis2] += p2 + margin[axis2];

                    testItem = item;
                    testItem.center = testCenter;

                    if (!container.Contains(testItem))
                    {
                        if (i1 == 0) 
                            return results;

                        break;
                    }

                    results.Add(testItem);
                }
            }
        }

        public static IReadOnlyList<Bounds> PackByThreeAxes(
            Bounds container,
            Bounds item,
            Axis firstAxis = Axis.X,
            Axis secondAxis = Axis.Y,
            Axis thirdAxis = Axis.Z,
            Vector3 margin = default
            )
        {
            var axis1 = (int)firstAxis;
            var axis2 = (int)secondAxis;
            var axis3 = (int)thirdAxis;

            if (margin != Vector3.zero)
            {
                var itemSize = item.size;

                itemSize[axis1] += itemSize[axis1] * Mathf.Max(margin[axis1], 0f);
                itemSize[axis2] += itemSize[axis2] * Mathf.Max(margin[axis2], 0f);
                itemSize[axis3] += itemSize[axis3] * Mathf.Max(margin[axis3], 0f);

                item.size = itemSize;
            }

            var startCenter = container.min + item.extents;

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
                            return results;

                        p1 = i1 * item.size[axis1];

                        testCenter = startCenter;
                        testCenter[axis1] += p1 + margin[axis1];
                        testCenter[axis2] += p2 + margin[axis2];
                        testCenter[axis3] += p3 + margin[axis3];

                        testItem = item;
                        testItem.center = testCenter;

                        if (!container.Contains(testItem))
                        {
                            if (i1 != 0) break;
                            else if (i3 != 0) breakI3 = true;
                            else return results;

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

        //private readonly struct PackCacheKey
        //{
        //    public readonly Bounds Container;
        //    public readonly Bounds Item;
        //    public readonly Vector3 Margin;
        //    public readonly Axis Axis1;
        //    public readonly Axis? Axis2;
        //    public readonly Axis? Axis3;

        //    public PackCacheKey(Bounds container, Bounds item, Vector3 margin, Axis axis1, Axis? axis2, Axis? axis3)
        //    {
        //        Container = container;
        //        Item = item;
        //        Margin = margin;
        //        Axis1 = axis1;
        //        Axis2 = axis2;
        //        Axis3 = axis3;
        //    }
        //}
    }
}
