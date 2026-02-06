using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

#nullable enable

namespace CCEnvs.Unity.Collections
{
    public readonly struct MapInt<T> : IEnumerable<T>
    {
        private readonly Dictionary<Vector3Int, T> collection;

        public readonly T this[Vector3Int pos] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => collection[pos];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => collection[pos] = value;
        }

        public readonly T this[Vector2Int pos] {
            get => this[(Vector3Int)pos];
        }
        public readonly T this[int x, int y, int z] {
            get => this[new Vector3Int(x, y, z)];
        }
        public readonly T this[int x, int y] => this[new Vector3Int(x, y)];

        public readonly BoundsInt Bounds { get; }
        public readonly int Count {
            get
            {
                Vector3Int size = Bounds.size;
                return size.x * size.y * size.z;
            }
        }

        public MapInt(BoundsInt bounds)
        {
            Bounds = bounds;
            collection = new Dictionary<Vector3Int, T>(bounds.size.x * bounds.size.y * (bounds.size.z + 1));
        }

        public readonly bool Contains() => Count > 0;
        public readonly bool Contains(T? item)
        {
            throw new System.NotImplementedException();
        }
        public readonly bool Contains(Vector3Int pos)
        {
            throw new System.NotImplementedException();
        }
        public readonly bool Contains(Vector2Int pos)
        {
            return Contains((Vector3Int)pos);
        }

        public readonly IEnumerator<T> GetEnumerator() => collection.Values.GetEnumerator();

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}