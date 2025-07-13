using System;
using System.Collections;
using System.Collections.Generic;
using UTIRLib.Diagnostics;
using UTIRLib.Extensions;

#nullable enable

namespace UTIRLib.Patterns.Composite
{
    public class Composite<T> : ReadOnlyComposite<T>, IComposite<T>
    {
        protected new readonly List<T> childs = new();

        public Composite()
        { }

        public Composite(T[] childs) => AddRange(childs);

        public void Add(object value) => Add(value.Convert<T>()!);

        /// <exception cref="ArgumentException"></exception>
        public void Add(T value)
        {
            if (value.IsDefault())
            {
                throw new ArgumentException(nameof(value));
            }

            childs.Add(value);
            childsCount = childs.Count;
        }

        public void Remove(object? value) => Remove(value.Is<T>()!);

        public void Remove(T? value)
        {
            if (value.IsDefault())
            {
                return;
            }

            childs.Remove(value);
            childsCount = childs.Count;
        }

        public void AddRange(params object[] values) => AddRange(values.Convert<T[]>()!);

        public void AddRange(params T[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                Add(values[i]);
            }
        }

        public void RemoveRange(params object?[] values) => RemoveRange(values.Convert<T[]>()!);

        public void RemoveRange(params T?[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                Remove(values[i]);
            }
        }

        public override int GetHashCode() => HashCode.Combine(childs);

        public override bool Equals(object obj) => GetHashCode() == obj.GetHashCode();

        public static Composite<T> operator +(Composite<T> a, T b)
        {
            a.Add(b);
            return a;
        }

        public static Composite<T> operator -(Composite<T> a, T b)
        {
            a.Remove(b);
            return a;
        }

        public static bool operator ==(Composite<T> a, Composite<T> b) => a.Equals(b);

        public static bool operator !=(Composite<T> a, Composite<T> b) => !a.Equals(b);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}