using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UTIRLib.UExtensions;

#nullable enable

namespace UTIRLib.Collections
{
    public record Map<T> : IEnumerable<T>
    {
        private readonly T[,] values;
        private readonly int width;
        private readonly int height;
        private readonly int xOffset;
        private readonly int yOffset;
        private readonly int minX;
        private readonly int minY;
        private readonly int maxX;
        private readonly int maxY;

        public int Width => width;
        public int Height => height;
        public int Count => width * height;
        public int MinX => minX;
        public int MinY => minY;
        public int MaxX => maxX;
        public int MaxY => maxY;
        public T this[int x, int y] {
            get => GetValue(x, y);
            set => SetValue(x, y, value);
        }
        public T this[Vector2Int position] {
            get => GetValue(position);
            set => SetValue(position.x, position.y, value);
        }
        public T this[Vector3Int position] {
            get => GetValue(position);
            set => SetValue(position.x, position.y, value);
        }

        public Map(int width, int height, int minX = 0, int minY = 0)
        {
            values = new T[width, height];

            xOffset = width / 2;
            yOffset = height / 2;

            this.width = width;
            this.height = height;

            this.minX = minX;
            this.minY = minY;

            maxX = width + minX - 1;
            maxY = height + minY - 1;
        }
        public Map(int capacity, int minX = 0, int minY = 0) : this(capacity, capacity, minX, minY)
        {
        }
        public Map(Vector2Int size, int minX = 0, int minY = 0) : this(size.x, size.y, minX, minY)
        {
        }
        public Map(Vector3Int size, int minX = 0, int minY = 0) : this(size.ToVector2(), minX, minY)
        {
        }
        public Map(BoundsInt bounds) : this(bounds.size, bounds.xMin, bounds.yMin)
        {
        }

        public T GetValue(int x, int y) => GetValueInternal(x, y, doCheckPosition: true);
        public T GetValue(Vector2Int position) => GetValueInternal(position.x, position.y, doCheckPosition: true);
        public T GetValue(Vector3Int position) => GetValueInternal(position.x, position.y, doCheckPosition: true);

        public bool TryGetValue(int x, int y, [NotNullWhen(true)] out T value)
        {
            if (Contains(x, y))
            {
                value = GetValueInternal(x, y, doCheckPosition: false)!;

                return !EqualityComparer<T>.Default.Equals(value, default!);
            }

            value = default!;
            return false;
        }
        public bool TryGetValue(Vector2Int position, [NotNullWhen(true)] out T value)
        {
            if (Contains(position))
            {
                value = GetValueInternal(position.x, position.y, doCheckPosition: false)!;

                return !EqualityComparer<T>.Default.Equals(value, default!);
            }

            value = default!;
            return false;
        }
        public bool TryGetValue(Vector3Int position, [NotNullWhen(true)] out T value) =>
            TryGetValue(position.ToVector2(), out value);

        public void SetValue(int x, int y, T value) => SetValueInternal(x, y, value);
        public void SetValue(Vector2Int position, T value) => SetValueInternal(position.x, position.y, value);
        public void SetValue(Vector3Int position, T value) => SetValueInternal(position.x, position.y, value);

        /// <exception cref="WrongCollectionException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Fill(Vector2Int[] positions, T[] values)
        {
            if (positions.IsNullOrEmpty())
            {
                throw new ArgumentException($"{nameof(positions)} is null or empty.");
            }
            if (values.IsNullOrEmpty())
            {
                throw new ArgumentException($"{nameof(values)} is null or empty.");
            }
            if (positions.Length != values.Length)
            {
                throw new ArgumentException("Arrays must be the same size.");
            }

            for (int i = 0; i < positions.Length; i++)
            {
                SetValue(positions[i], values[i]);
            }
        }
        public void Fill(Vector3Int[] positions, T[] values) =>
            Fill(positions.Select((pos) => pos.ToVector2()).ToArray(), values);
        public void Fill(T value)
        {
            for (int y = minY; y < maxY; y++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    values[x, y] = value;
                }
            }
        }

        public void Clear() => Fill(default!);

        public bool Contains(T value)
        {
            for (int y = minY; y < maxY; y++)
            {
                for (int x = minX; x < maxX; x++)
                {
                    if (!EqualityComparer<T>.Default.Equals(values[x, y], default!) && values[x, y]!.Equals(value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        public bool Contains(int x, int y) => x < maxX && x > minX && y < maxY && y > minY;
        public bool Contains(Vector2Int position) => Contains(position.x, position.y);
        public bool Contains(Vector3Int position) => Contains(position.x, position.y);

        public IEnumerator<T> GetEnumerator() => new DefaultEnumerator<T>(values);

        private T GetValueInternal(int x, int y, bool doCheckPosition = false)
        {
            if (doCheckPosition && !Contains(x, y))
            {
                throw new IndexOutOfRangeException($"Wasn't found position: x = {x}, y = {y}");
            }

            return values[x + xOffset, y + yOffset];
        }

        private void SetValueInternal(int x, int y, T value, bool doCheckPosition = false)
        {
            if (doCheckPosition && !Contains(x, y))
            {
                throw new IndexOutOfRangeException($"Wasn't found position: x = {x}, y = {y}");
            }

            values[x + xOffset, y + yOffset] = value;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}