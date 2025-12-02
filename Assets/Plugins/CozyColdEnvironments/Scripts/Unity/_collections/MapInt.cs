using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace CCEnvs.Unity.Collections
{
    public readonly struct MapInt<T> : IEnumerable<T>
    {
        private readonly T[,,] values;

        public readonly T this[Vector3Int pos] {
            get
            {
                pos -= Bounds.min;
                return values[pos.x, pos.y, pos.z];
            }
            set
            {
                pos -= Bounds.min;
                values[pos.x, pos.y, pos.z] = value;
            }
        }

        public readonly T this[Vector2Int pos] => this[(Vector3Int)pos];
        public readonly T this[int x, int y, int z] => this[new Vector3Int(x, y, z)];
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

            values = ArrayFromBounds(bounds);
        }

        public readonly bool Contains() => Count > 0;
        public readonly bool Contains(T? item)
        {
            var equalityComparer = EqualityComparer<T?>.Default;
            foreach (var arrItem in values)
            {
                if (equalityComparer.Equals(item, arrItem))
                    return true;
            }

            return false;
        }
        public readonly bool Contains(Vector3Int pos)
        {
            return Bounds.Contains(pos); 
        }
        public readonly bool Contains(Vector2Int pos)
        {
            return Bounds.Contains((Vector3Int)pos);
        }

        public static T[,,] ArrayFromBounds(BoundsInt bounds)
        {
            var size = bounds.size;
            return new T[size.x, size.y, size.z];
        }

        public readonly IEnumerator<T> GetEnumerator() => values.To<IEnumerable<T>>().GetEnumerator();

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}