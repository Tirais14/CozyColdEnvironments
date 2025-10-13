#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#pragma warning disable S3267
#pragma warning disable S2368
namespace CCEnvs.Collections
{
    public class Table<T> : ITable<T>
    {
        private readonly ImmutableTable<T> inner;
        private readonly Action<int, int, T> valueSetter;

        public T this[int colIdx, int rowIdx] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => inner[colIdx, rowIdx];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => valueSetter(colIdx, rowIdx, value); }

        public T this[TablePointer.Pos pos] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => inner[pos];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => this[pos.Column, pos.Row] = value; }

        public T this[TablePointer pointer] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => inner[pointer];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => this[pointer.Current] = value;
        }

        public int ColumnCount { get; }
        public int RowCount { get; }
        public int Count => ColumnCount * RowCount;

        public Table(Func<int, int, T> valueGetter,
             Action<int, int, T> valueSetter,
             TablePointer pointer)
        {
            inner = new ImmutableTable<T>(valueGetter, pointer);
            this.valueSetter = valueSetter;
        }
        public Table(T[,] array)
            :
            this((x, y) => array[x, y],
                 (x, y, value) => array[x, y] = value,
                 TablePointer.From(array))
        {

        }
        public Table(int columnCount, int rowCount)
            :
            this(new T[columnCount, rowCount])
        {
        }

        public void Clear()
        {
            foreach (var pos in inner.GetTablePointer())
                this[pos] = default!;
        }

        public IEnumerable<(TablePointer.Pos pos, T value)> AsPositionedEnumerable()
        {
            foreach (var pos in GetTablePointer())
                yield return (pos, inner[pos]);
        }

        public IEnumerable<(int col, int row, T value)> AsIndexedEnumerable()
        {
            foreach (var item in AsPositionedEnumerable())
                yield return (item.pos.Column, item.pos.Row, item.value);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in inner)
            {
                if (item.IsNotDefault())
                    yield return item;
            }
        }

        public TablePointer GetTablePointer() => inner.GetTablePointer();

        public bool Remove(T value)
        {
            if (Equals(value, default(T)))
                return false;

            try
            {
                this[GetTablePointer().First(x => Equals(x, value))] = default!;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void RemoveAt(int colIdx, int rowIdx)
        {
            this[colIdx, rowIdx] = default!;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
