using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace CCEnvs.Unity.Collections
{
    public readonly struct MapInt<T> : IEnumerable<T>
    {
        private readonly T[,,] values;

        public readonly BoundsInt Bounds { get; }
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
        public readonly T this[Vector2Int pos] => this[pos.ToVector3()];
        public readonly T this[int x, int y, int z] => this[new Vector3Int(x, y, z)];
        public readonly T this[int x, int y] => this[new Vector3Int(x, y)];

        public MapInt(BoundsInt bounds)
        {
            Bounds = bounds;

            values = ArrayFromBounds(bounds);
        }

        public static T[,,] ArrayFromBounds(BoundsInt bounds)
        {
            var size = bounds.size;
            return new T[size.x, size.y, size.z];
        }

        public readonly IEnumerator<T> GetEnumerator() => values.As<IEnumerable<T>>().GetEnumerator();

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}