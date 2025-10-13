#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace CCEnvs.Collections
{
    public struct TablePointer : IEnumerator<TablePointer.Pos>, IEnumerable<TablePointer.Pos>
    {
        public int ColumnCount { get; }
        public int RowCount { get; }
        public int ColumnOffset { get; }
        public int RowOffset { get; }
        public int ColumnStepMultiplier { get; }
        public int RowStepMultiplier { get; }
        public int Column { get; private set; }
        public int Row { get; private set; }

        public readonly int Count => RowCount * ColumnCount;
        public readonly Pos Current {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new(Column, Row);
        }

        readonly object IEnumerator.Current => Current;

        public TablePointer(int columnCount,
                            int rowCount,
                            int columnOffset,
                            int rowOffset,
                            int columnStepMultiplier,
                            int rowStepMultiplier)
            :
            this()
        {
            if (columnStepMultiplier < 1)
                throw new ArgumentException(columnStepMultiplier.ToString(), nameof(columnStepMultiplier));
            if (rowStepMultiplier < 1)
                throw new ArgumentException(rowStepMultiplier.ToString(), nameof(rowStepMultiplier));

            ColumnCount = columnStepMultiplier * columnCount;
            RowCount = rowStepMultiplier * rowCount;

            ColumnOffset = columnOffset;
            RowOffset = rowOffset;

            ColumnStepMultiplier = columnStepMultiplier;
            RowStepMultiplier = rowStepMultiplier;

            Reset();
        }

        public TablePointer(int columnCount,
                            int rowCount,
                            int columnOffset,
                            int rowOffset)
            :
            this(columnCount,
                 rowCount,
                 columnOffset,
                 rowOffset,
                 columnStepMultiplier: 1,
                 rowStepMultiplier: 1)
        {
        }

        public TablePointer(int columnCount,
                            int rowCount,
                            int columnStepMultiplier,
                            int rowStepMultiplier,
                            bool _)
            :
            this(columnCount,
                 rowCount,
                 columnOffset: 0,
                 rowOffset: 0,
                 columnStepMultiplier,
                 rowStepMultiplier)
        {
        }

        public TablePointer(int columnCount, int rowCount)
            :
            this(columnCount,
                 rowCount,
                 columnOffset: 0,
                 rowOffset: 0,
                 columnStepMultiplier: 1,
                 rowStepMultiplier: 1)
        {
        }

        public static TablePointer From<T>(T[,] array)
        {
            return new TablePointer(array.GetLength(0), array.GetLength(1));
        }

        public bool MoveNext()
        {
            Column += ColumnStepMultiplier;

            if (Column >= ColumnCount)
            {
                Column = ColumnOffset;
                Row += RowStepMultiplier;
            }

            return Row < RowCount;
        }

        public void Reset()
        {
            Column = ColumnOffset - ColumnStepMultiplier;
            Row = RowOffset;
        }

        public readonly IEnumerator<Pos> GetEnumerator() => this;

        readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        readonly void IDisposable.Dispose()
        {
        }

        public readonly struct Pos
        {
            public int Column { get;}
            public int Row { get; }

            public Pos(int column, int row)
            {
                Column = column;
                Row = row;
            }
        }
    }
}
