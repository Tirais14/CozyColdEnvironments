using System;
using System.Collections;
using System.Collections.Generic;

#nullable enable

namespace UTIRLib.Collections
{
    public sealed class Flags<T> : ICollection<T>, IEquatable<Flags<T>>
    {
        private readonly HashSet<T> values = new();

        public int Count => values.Count;
        public bool IsReadOnly => false;

        /// <summary>
        /// If set to true, the <see cref="Contains(T)"/> method returns false if the item is in the collection, otherwise true.
        /// </summary>
        public bool Inverted { get; set; }

        public bool this[T item] => values.Contains(item);

        private Flags(IEnumerable<T>? toAddValues, bool inverted, int capacity)
        {
            if (toAddValues == null)
            {
                if (capacity > 0)
                {
                    values = new HashSet<T>(capacity);
                }
                else values = new HashSet<T>();
            }
            else values = new HashSet<T>(toAddValues);

            Inverted = inverted;
        }

        public Flags()
        {
        }

        public Flags(bool inverted) : this(toAddValues: null,
                                           inverted,
                                           capacity: -1)
        {
        }

        public Flags(IEnumerable<T> toAddValues, bool inverted = false) : this(toAddValues,
                                                                               inverted,
                                                                               capacity: -1)
        {
        }

        public Flags(int capacity) : this(toAddValues: null,
                                          inverted: false,
                                          capacity)
        {
        }

        public Flags(int capacity, bool inverted) : this(toAddValues: null,
                                                         inverted,
                                                         capacity)
        {
        }

        public void Add(T item) => values.Add(item);

        public bool TryAdd(T item)
        {
            if (values.TryGetValue(item, out _))
                return false;

            values.Add(item);

            return true;
        }

        public void Clear() => values.Clear();

        public bool Contains(T item)
        {
            if (Inverted) return !values.Contains(item);

            return values.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var item in values)
            {
                array[arrayIndex++] = item;
            }
        }

        public bool Remove(T item) => values.Remove(item);

        public IEnumerator<T> GetEnumerator() => values.GetEnumerator();

        public override int GetHashCode()
        {
            HashCode hashCode = new();

            hashCode.Add(values);
            hashCode.Add(Inverted);

            return hashCode.ToHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Flags<T> otherObj)
            {
                Equals(otherObj);
            }

            return false;
        }

        public bool Equals(Flags<T> other)
        {
            if (Count != other.Count)
            {
                return false;
            }

            foreach (var item in other)
            {
                if (!Contains(item)) return false;
            }

            return true;
        }

        public override string ToString() => values.ToString() + (Inverted ? " | Inverted" : null);

        public static Flags<T> operator +(Flags<T> flags, T item)
        {
            flags.Add(item);

            return flags;
        }

        public static Flags<T> operator +(Flags<T> flags, IEnumerable<T> collection)
        {
            flags.AddRange(collection);

            return flags;
        }

        public static Flags<T> operator -(Flags<T> flags, T item)
        {
            flags.Remove(item);

            return flags;
        }

        public static Flags<T> operator -(Flags<T> flags, IEnumerable<T> collection)
        {
            flags.RemoveRange(collection);

            return flags;
        }

        public static bool operator ==(Flags<T> a, Flags<T> b) => a.Equals(b);

        public static bool operator !=(Flags<T> a, Flags<T> b) => !a.Equals(b);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}