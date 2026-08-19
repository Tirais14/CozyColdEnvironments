using CCEnvs.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
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
            private readonly NativeArray3D<T> array;

            public T this[int index] => array[index];

            public T this[int x, int y, int z] {
                get => array[x, y, z];
            }

            public T this[int3 pos] {
                get => array[pos];
            }

            public int Length => array.Length;

            public int3 Size => array.Size;

            public bool IsCreated => array.IsCreated;

            public ReadOnly(in NativeArray3D<T> array)
            {
                this.array = array;
            }

            public static bool operator ==(in ReadOnly left, in ReadOnly right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(in ReadOnly left, in ReadOnly right)
            {
                return !(left == right);
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
                return array.Equals(other.array);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(array);
            }

            public IEnumerator<T> GetEnumerator()
            {
                return array.array.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private NativeArray<T> array;

        public T this[int index] {
            get => array[index];
        }

        public T this[int x, int y, int z] {
            get => array[CalculateIndex(x, y, z)];
            set => array[CalculateIndex(x, y, z)] = value;
        }

        public T this[int3 position] {
            get => this[position.x, position.y, position.z];
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

        private readonly int CalculateIndex(int x, int y, int z)
        {
            return Size.x + x * (Size.y + y * z);
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
