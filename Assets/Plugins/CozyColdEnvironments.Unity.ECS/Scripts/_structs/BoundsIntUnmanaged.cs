using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

#nullable enable
namespace CCEnvs.UnityX.ECS
{
    [BurstCompile]
    public readonly struct BoundsIntUnmanaged
    {
        public struct Enumerator
        {
            private readonly BoundsIntUnmanaged bounds;

            private int x;
            private int y;
            private int z;

            public int3 Current { get; private set; }

            public Enumerator(in BoundsIntUnmanaged bounds)
            {
                this.bounds = bounds;
                Current = default;

                x = bounds.Position.x - 1;
                y = bounds.Position.y - 1;
                z = bounds.Position.z - 1;
            }

            public bool MoveNext()
            {
                x++;
                if (x >= bounds.XMin && x <= bounds.XMax)
                {
                    Current = new int3(x, y, z);
                    return true;
                }

                z++;
                if (z >= bounds.ZMin && z <= bounds.ZMax)
                {
                    Current = new int3(x, y, z);
                    return true;
                }

                y++;
                if (y >= bounds.YMin && y <= bounds.YMax)
                {
                    Current = new int3(x, y, z);
                    return true;
                }

                return false;
            }
        }

        public int3 Position { get; }
        public int3 Size { get; }

        public int XMin => Position.x;
        public int YMin => Position.y;
        public int ZMin => Position.z;

        public int XMax => Position.x + Size.x;
        public int YMax => Position.y + Size.y;
        public int ZMax => Position.z + Size.z;

        public BoundsIntUnmanaged(int3 position, int3 size)
        {
            Size = new int3(math.max(size.x, 0), math.max(size.y, 0), math.max(size.z, 0));
            Position = position;
        }

        [BurstCompile]
        public bool Contains(int3 position)
        {
            if (Size.Equals(int3.zero))
                return false;

            return position.x >= XMin &&
                   position.x <= XMax &&
                   position.y >= YMin &&
                   position.y <= YMax &&
                   position.z >= ZMin &&
                   position.z <= ZMax;
        }

        [BurstCompile]
        public NativeArray<int3> ToNativeArray(AllocatorManager.AllocatorHandle allocator)
        {
            var points = new NativeList<int3>(Size.x * Size.y * Size.z, allocator);

            foreach (var point in this)
                points.Add(point);

            return points.AsArray();
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }
    }
}
