using CCEnvs.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

#nullable enable
namespace CCEnvs.UnityX.ECS.Collections
{
    public struct NativeArray3D<T> 
        : 
        IEnumerable<T>,
        IDisposable, 
        INativeDisposable,
        IEquatable<NativeArray3D<T>>
        
        where T : struct
    {
        public readonly struct ReadOnly : IEnumerable<T>, IEquatable<ReadOnly>
        {
            private readonly NativeArray3D<T> core;

            public T this[int index] => core[index];

            public T this[int x, int y, int z] {
                get => core[x, y, z];
            }

            public T this[int3 pos] {
                get => core[pos];
            }

            public int Length => core.Length;

            public int3 Size => core.Size;

            public bool IsCreated => core.IsCreated;

            public ReadOnly(in NativeArray3D<T> array)
            {
                this.core = array;
            }

            public static bool operator ==(in ReadOnly left, in ReadOnly right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(in ReadOnly left, in ReadOnly right)
            {
                return !(left == right);
            }

            public unsafe ref T ElementAt(int x, int y, int z)
            {
                var arrayPtr = core.array.GetUnsafePtr();

                if (x <= -1 || x >= Size.x)
                    throw CC.ThrowHelper.IndexOutOfRangeException(x, nameof(x));
                if (y <= -1 || y >= Size.y)
                    throw CC.ThrowHelper.IndexOutOfRangeException(y, nameof(y));
                if (z <= -1 || z >= Size.z)
                    throw CC.ThrowHelper.IndexOutOfRangeException(z, nameof(z));

                return ref UnsafeUtility.ArrayElementAsRef<T>(arrayPtr, CalculateIndex(x, y, z));
            }

            public readonly int CalculateIndex(int x, int y, int z)
            {
                return core.CalculateIndex(x, y, z);
            }

            public readonly int CalculateIndex(int3 position)
            {
                return core.CalculateIndex(position);
            }

            public readonly T GetValue(int x, int y, int z)
            {
                return core.GetValue(x, y, z);
            }
            public readonly T GetValue(int3 position)
            {
                return core.GetValue(position);
            }

            public readonly bool TryGetValue(int x, int y, int z, out T result)
            {
                if (!IsInBounds(x, y, z))
                {
                    result = default;
                    return false;
                }

                result = this[x, y, z];
                return true;
            }
            public readonly bool TryGetValue(int3 position, out T result)
            {
                return TryGetValue(position.x, position.y, position.z, out result);
            }

            public readonly bool IsInBounds(int x, int y, int z)
            {
                return x < Size.x &&
                       y < Size.y &&
                       z < Size.z;
            }
            public readonly bool IsInBounds(int3 position)
            {
                return IsInBounds(position.x, position.y, position.z);
            }

            public override bool Equals(object? obj)
            {
                return obj is ReadOnly only && Equals(only);
            }

            public bool Equals(ReadOnly other)
            {
                return core.Equals(other.core);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(core);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return core.array.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private NativeArray<T> array;

        public readonly T this[int index] {
            get => array[index];
        }

        public T this[int x, int y, int z] {
            readonly get => GetValue(x, y, z);
            set => SetValue(x, y, z, value);
        }

        public T this[int3 position] {
            readonly get => this[position.x, position.y, position.z];
            set => this[position.x, position.y, position.z] = value;
        }

        public int Length => array.IsCreated ? array.Length : 0;

        public int3 Size { get; }

        public readonly bool IsCreated => array.IsCreated;

        public NativeArray3D(int3 size, Allocator allocator)
        {
            Size = math.max(size, new int3(0));
            array = new NativeArray<T>(Size.x * Size.y * Size.z, allocator);
        }

        public NativeArray3D(int xSize, int ySize, int zSize, Allocator allocator)
        {
            Size = new int3(math.max(xSize, 0), math.max(ySize, 0), math.max(zSize, 0));
            array = new NativeArray<T>(Size.x * Size.y * Size.z, allocator);
        }

        public static bool operator ==(in NativeArray3D<T> left, in NativeArray3D<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(in NativeArray3D<T> left, in NativeArray3D<T> right)
        {
            return !(left == right);
        }

        public readonly int CalculateIndex(int x, int y, int z)
        {
            return x + y * Size.x + z * Size.x * Size.y;
        }

        public readonly int CalculateIndex(int3 position)
        {
            return CalculateIndex(position.x, position.y, position.z);
        }

        public void SetValue(int x, int y, int z, T value)
        {
            if (x <= -1 || x >= Size.x)
                throw CC.ThrowHelper.IndexOutOfRangeException(x, nameof(x));
            if (y <= -1 || y >= Size.y)
                throw CC.ThrowHelper.IndexOutOfRangeException(y, nameof(y));
            if (z <= -1 || z >= Size.z)
                throw CC.ThrowHelper.IndexOutOfRangeException(z, nameof(z));

            array[CalculateIndex(x, y, z)] = value;
        }
        public void SetValue(int3 position, T value)
        {
            SetValue(position.x, position.y, position.z, value);
        }

        public readonly T GetValue(int x, int y, int z)
        {
            if (x <= -1 || x >= Size.x)
                throw CC.ThrowHelper.IndexOutOfRangeException(x, nameof(x));
            if (y <= -1 || y >= Size.y)
                throw CC.ThrowHelper.IndexOutOfRangeException(y, nameof(y));
            if (z <= -1 || z >= Size.z)
                throw CC.ThrowHelper.IndexOutOfRangeException(z, nameof(z));

            return array[CalculateIndex(x, y, z)];
        }
        public readonly T GetValue(int3 position)
        {
            return GetValue(position.x, position.y, position.z);
        }

        public readonly bool TryGetValue(int x, int y, int z, out T result)
        {
            if (!IsInBounds(x, y, z))
            {
                result = default;
                return false;
            }

            result = this[x, y, z];
            return true;
        }
        public readonly bool TryGetValue(int3 position, out T result)
        {
            return TryGetValue(position.x, position.y, position.z, out result);
        }

        public readonly bool IsInBounds(int x, int y, int z)
        {
            return x < Size.x &&
                   x >= 0 &&
                   y < Size.y && 
                   y >= 0 &&
                   z < Size.z &&
                   z >= 0;
        }
        public readonly bool IsInBounds(int3 position)
        {
            return IsInBounds(position.x, position.y, position.z);
        }

        public void Dispose() => array.Dispose();
        public JobHandle Dispose(JobHandle inputDeps)
        {
            return array.Dispose(inputDeps);
        }

        public T[] ToArray() => array.ToArray();

        public readonly ReadOnly AsReadOnly()
        {
            return new ReadOnly(this);
        }

        public override bool Equals(object? obj)
        {
            return obj is NativeArray3D<T> d && Equals(d);
        }

        public bool Equals(NativeArray3D<T> other)
        {
            return array.Equals(other.array);
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(array);
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (!IsCreated)
                return Array.Empty<T>().GetEnumeratorT();

            return array.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static class NativeArray3DExtensions
    {
        public static int IndexOf<T>(this in NativeArray3D<T> source, T item)
            where T : struct, IEquatable<T>
        {
            if (source.IsCreated)
            {
                for (int i = 0; i < source.Length; i++)
                    if (source[i].Equals(item))
                        return i;
            }

            return -1;
        }
        public static int IndexOf<T>(this in NativeArray3D<T>.ReadOnly source, T item)
            where T : struct, IEquatable<T>
        {
            if (source.IsCreated)
            {
                for (int i = 0; i < source.Length; i++)
                    if (source[i].Equals(item))
                        return i;
            }

            return -1;
        }

        public static bool Contains<T>(this in NativeArray3D<T> source, T item) where T : struct, IEquatable<T>
        {
            return source.IndexOf(item) != -1;
        }
        public static bool Contains<T>(this in NativeArray3D<T>.ReadOnly source, T item) where T : struct, IEquatable<T>
        {
            return source.IndexOf(item) != -1;
        }
    }
}
