using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityEngine
{
    // Represents an axis aligned bounding box with all values as integers in 2D space.
    [Serializable]
    public struct Bounds2Int : IEquatable<Bounds2Int>, IFormattable
    {
        // An iterator that allows you to iterate over all positions within the Bounds2Int.
        public struct PositionEnumerator : IEnumerator<Vector2Int>, IEnumerator, IDisposable
        {
            private readonly Vector2Int _min;
            private readonly Vector2Int _max;
            private Vector2Int _current;

            // Current position of the enumerator.
            public readonly Vector2Int Current {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return _current;
                }
            }

            readonly object IEnumerator.Current {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    return Current;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public PositionEnumerator(in Vector2Int min, in Vector2Int max)
            {
                _min = min;
                _max = max;
                _current = _min;
                _current.x--;
            }

            // Returns this as an iterator that allows you to iterate over all positions within the Bounds2Int.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public readonly PositionEnumerator GetEnumerator()
            {
                return this;
            }

            // Moves the enumerator to the next position.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                if (_current.y >= _max.y)
                {
                    return false;
                }

                _current.x++;
                if (_current.x >= _max.x)
                {
                    _current.x = _min.x;
                    if (_current.x >= _max.x)
                    {
                        return false;
                    }

                    _current.y++;
                    if (_current.y >= _max.y)
                    {
                        return false;
                    }
                }

                return true;
            }

            // Resets this enumerator to its starting state.
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Reset()
            {
                _current = _min;
                _current.x--;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly void  IDisposable.Dispose()
            {
            }
        }

        [SerializeField]
        private Vector2Int m_Position;

        [SerializeField]
        private Vector2Int m_Size;

        // X value of the minimal point of the box.
        public int x {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get
            {
                return m_Position.x;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                m_Position.x = value;
            }
        }

        // Y value of the minimal point of the box.
        public int y {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get
            {
                return m_Position.y;
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                m_Position.y = value;
            }
        }

        // The center of the bounding box.
        public readonly Vector2 center {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new Vector2
                {
                    x = (float)m_Position.x + (float)m_Size.x * 0.5f,
                    y = (float)m_Position.y + (float)m_Size.y * 0.5f
                };
            }
        }

        // The minimal point of the box.
        public Vector2Int min {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get
            {
                return new Vector2Int(xMin, yMin);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                SetMinMax(value, max);
            }
        }

        // The maximal point of the box.
        public Vector2Int max {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get
            {
                return new Vector2Int(xMax, yMax);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                SetMinMax(min, value);
            }
        }

        // The X value of the minimal point of the box.
        public int xMin {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => m_Position.x;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_Position.x = value;
        }

        // The Y value of the minimal point of the box.
        public int yMin {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => m_Position.y;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_Position.y = value;
        }

        // The X value of the maximal point of the box.
        public int xMax {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => m_Position.x + m_Size.x;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_Size.x = value - m_Position.x;
        }

        // The Y value of the maximal point of the box.
        public int yMax {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => m_Position.y + m_Size.y;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_Size.y = value - m_Position.y;
        }

        // The width of the box.
        public int width {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => m_Size.x;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_Size.x = value;
        }

        // The height of the box.
        public int height {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => m_Size.y;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => m_Size.y = value;
        }

        public readonly Vector2Int position => m_Position;

        // Constructor to initialize the bounding box with position and size.
        public Bounds2Int(Vector2Int position, Vector2Int size)
        {
            m_Position = position;
            m_Size = size;
        }

        // Constructor to initialize the bounding box with min and max points.
        public Bounds2Int(in Vector2Int position, in Vector2Int size)
        {
            m_Position = position;
            m_Size = size;
        }

        // Sets the bounds based on min and max points.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetMinMax(Vector2Int minPosition, Vector2Int maxPosition)
        {
            m_Position = minPosition;
            m_Size = maxPosition - minPosition;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetMinMax(in Vector2Int minPosition, in Vector2Int maxPosition)
        {
            m_Position = minPosition;
            m_Size = maxPosition - minPosition;
        }

        // Clamps the position and size of this bounding box to the given bounds.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClampToBounds(Bounds2Int bounds)
        {
            int num = bounds.xMax;
            int num2 = bounds.yMax;
            int num3 = Math.Max(Math.Min(num, m_Position.x), bounds.xMin);
            int num4 = Math.Max(Math.Min(num2, m_Position.y), bounds.yMin);
            m_Size.Set(Math.Min(num - num3, m_Size.x), Math.Min(num2 - num4, m_Size.y));
            m_Position.Set(num3, num4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClampToBounds(in Bounds2Int bounds)
        {
            int num = bounds.xMax;
            int num2 = bounds.yMax;
            int num3 = Math.Max(Math.Min(num, m_Position.x), bounds.xMin);
            int num4 = Math.Max(Math.Min(num2, m_Position.y), bounds.yMin);
            m_Size.Set(Math.Min(num - num3, m_Size.x), Math.Min(num2 - num4, m_Size.y));
            m_Position.Set(num3, num4);
        }

        // Is point contained in the bounding box?
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(Vector2Int position)
        {
            int num = position.x;
            int num2 = position.y;
            return num >= xMin && num < xMax && num2 >= yMin && num2 < yMax;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Contains(in Vector2Int position)
        {
            int num = position.x;
            int num2 = position.y;
            return num >= xMin && num < xMax && num2 >= yMin && num2 < yMax;
        }

        // Returns a formatted string for the bounds.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly string ToString()
        {
            return ToString(null, null);
        }

        // Returns a formatted string for the bounds.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly string ToString(string format)
        {
            return ToString(format, null);
        }

        // Returns a formatted string for the bounds.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly string ToString(string format, IFormatProvider formatProvider)
        {
            formatProvider ??= CultureInfo.InvariantCulture.NumberFormat;

            return $"Position: {m_Position.ToString(format, formatProvider)}, Size: {m_Size.ToString(format, formatProvider)}";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Bounds2Int lhs, Bounds2Int rhs)
        {
            if (lhs.m_Position == rhs.m_Position)
            {
                return lhs.m_Size == rhs.m_Size;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Bounds2Int lhs, Bounds2Int rhs)
        {
            if (lhs.m_Position == rhs.m_Position)
            {
                return !(lhs.m_Size == rhs.m_Size);
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly bool Equals(object other)
        {
            if (other is Bounds2Int other2)
            {
                return Equals(in other2);
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(Bounds2Int other)
        {
            if (m_Position.Equals(in other.m_Position))
            {
                return m_Size.Equals(in other.m_Size);
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(in Bounds2Int other)
        {
            if (m_Position.Equals(in other.m_Position))
            {
                return m_Size.Equals(in other.m_Size);
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly int GetHashCode()
        {
            return m_Position.GetHashCode() ^ (m_Size.GetHashCode() << 2);
        }
    }
}