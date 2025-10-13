#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#pragma warning disable S2368
namespace CCEnvs.Collections
{
    public class ImmutableTable<T> : IImmutableTable<T>
    {
        private readonly TablePointer pointer;
        private readonly Func<int, int, T> valueGetter;

        public int ColumnCount => pointer.ColumnCount;
        public int RowCount => pointer.RowCount;

        public int Count {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => RowCount * ColumnCount;
        }

        public T this[int columnIdx, int rowIdx] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => valueGetter(columnIdx, rowIdx);
        }

        public T this[TablePointer.Pos pos] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this[pos.Column, pos.Row];
        }

        public T this[TablePointer pointer] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this[pointer.Current];
        }

        public ImmutableTable(Func<int, int, T> valueGetter, TablePointer pointer)
        {
            this.valueGetter = valueGetter;
            this.pointer = pointer;
        }
        public ImmutableTable(Func<int, int, T> valueGetter, int columnCount, int rowCount)
            :
            this(valueGetter, new TablePointer(columnCount, rowCount))
        {

        }
        public ImmutableTable(T[,] array)
            :
            this((x, y) => array[x, y], array.GetLength(0), array.GetLength(1))
        {
        }
        public ImmutableTable(int columnCount, int rowCount)
            :
            this(new T[columnCount, rowCount])
        {
        }

        public TablePointer GetTablePointer() => pointer;

        public IEnumerator<T> GetEnumerator()
        {
            return new TableEnumerator<T>(new TablePointer(ColumnCount, RowCount), this);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
