#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UTIRLib.Collections
{
    public readonly struct ArrayS<T> : IList<T>, ICloneable, IEquatable<ArrayS<T>>
    {
        public static ArrayS<T> Empty => new();

        private readonly T[]? array;

        public T[] Value => ToArray();
        public int Length => array.GetLengthOrZero();
        public bool IsReadOnly => array?.IsReadOnly ?? true;
        public bool IsEmpty => array.IsNullOrEmpty();

        int ICollection<T>.Count => array.GetLengthOrZero();

        public T this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                if (array is null)
                    throw new IndexOutOfRangeException("Array is empty.");

                return array[index];
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set {
                if (array is null)
                    throw new IndexOutOfRangeException("Array is empty.");

                array[index] = value;
            }
        }

        public ArrayS(IEnumerable<T>? values)
        {
            array = values?.ToArray();
        }

        public ArrayS(params T[]? values)
        {
            array = values;
        }

        public ArrayS(T[] a, T[] b)
        {
            array = a;
            Concat(b);
        }

        public ArrayS(T[] a, T item)
        {
            array = new T[a.Length + 1];

            array[a.Length] = item;
        }

        public ArrayS(IEnumerable<T> a, IEnumerable<T> b)
        {
            array = a.Concat(b).ToArray();
        }

        public ArrayS(int length)
        {
            array = new T[length];
        }

        public ArrayS(int length, int copyStartIndex, params T[] values)
        {
            array = new T[length];

            if (copyStartIndex < 0)
                copyStartIndex = 0;

            Array.Copy(values, sourceIndex: 0, array, copyStartIndex, values.Length);
        }

        public ArrayS(int length, params T[] values) : this(length, copyStartIndex: -1, values)
        {
        }

        public void Fill(T item) => For((x, i) => x[i] = item);

        public ArrayS<T> Resize(int newSize)
        {
            var resized = new T[newSize];

            int length = Math.Min(Value.Length, newSize);
            for (int i = 0; i < length; i++)
                resized[i] = array[i];

            return new ArrayS<T>(resized);
        }

        public ArrayS<T> Concat(T[] other)
        {
            if (IsEmpty && other.IsEmpty())
                return Empty;
            if (IsEmpty)
                return new ArrayS<T>((T[])other.Clone());
            if (other.IsEmpty())
                return new ArrayS<T>((T[])Value.Clone());

            var combined = new T[array!.Length + other.Length];


            return new ArrayS<T>(combined);
        }
        public ArrayS<T> Concat(ArrayS<T> other) => Concat(other.Value);

        public void For(Action<T[], int> action)
        {
            if (IsEmpty)
                return;

            int arrayLength = array!.Length;
            for (int i = 0; i < arrayLength; i++)
                action(array, i);
        }

        public void Foreach(Action<T> action)
        {
            if (IsEmpty)
                return;

            Span<T> arraySpan = new(array);
            int arrayLength = array!.Length;
            for (int i = 0; i < arrayLength; i++)
                action(arraySpan[i]);
        }

        public T? Find(Predicate<T> predicate)
        {
            if (IsEmpty)
                return default;

            Span<T> arraySpan = new(array);
            int arrayLength = array!.Length;
            for (int i = 0; i < arrayLength; i++)
            {
                if (predicate(arraySpan[i]))
                    return arraySpan[i];
            }

            return default;
        }

        public T[] ToArray() => array ?? Array.Empty<T>();

        public void RemoveAt(int index) => Value[index] = default!;

        public void Clear() => Array.Clear(Value, 0, Value.Length);

        public int IndexOf(T item) => Array.IndexOf(array, item);

        public bool Contains(T item) => IndexOf(item) != -1;

        public void CopyTo(T[] array, int arrayIndex) => Value.CopyTo(array, arrayIndex);

        public T[] Clone() => (T[])Value.Clone();

        public bool Equals(ArrayS<T> other)
        {
            return array is not null && other.array is not null && array == other.array;
        }
        public override bool Equals(object obj) => obj is ArrayS<T> arr && Equals(arr);

        public override string ToString() => Value.ToString();

        public override int GetHashCode() => HashCode.Combine(array);

        public static ArrayS<T> operator +(ArrayS<T> a, T[] b)
        {
            return a.Concat(b);
        }

        public static ArrayS<T> operator +(ArrayS<T> a, ArrayS<T> b)
        {
            return a.Concat(b);
        }

        public static implicit operator Array(ArrayS<T> array) => array.Value;

        public static implicit operator T[](ArrayS<T> array) => array.Value;

        public IEnumerator<T> GetEnumerator() => array.GetEnumeratorT();

        void ICollection<T>.Add(T item) => throw new NotSupportedException();

        bool ICollection<T>.Remove(T item) => throw new NotSupportedException();

        void IList<T>.Insert(int index, T item) => throw new NotSupportedException();

        object ICloneable.Clone() => Value.Clone();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
