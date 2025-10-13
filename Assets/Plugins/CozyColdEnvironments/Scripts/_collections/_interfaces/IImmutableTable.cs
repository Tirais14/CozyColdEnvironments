#nullable enable
using System.Collections;
using System.Collections.Generic;

namespace CCEnvs.Collections
{
    public interface IImmutableTable : IEnumerable
    {
        int ColumnCount { get; }
        int RowCount { get; }
        int Count { get; }

        object this[int colIdx, int rowIdx] { get; }
        object this[TablePointer.Pos pos] { get; }
        object this[TablePointer pointer] { get; }

        TablePointer GetTablePointer();
    }
    public interface IImmutableTable<out T> : IEnumerable<T>, IImmutableTable
    {
        new T this[int colIdx, int rowIdx] { get; }
        new T this[TablePointer.Pos pos] { get; }
        new T this[TablePointer pointer] { get; }

        object IImmutableTable.this[int colIdx, int rowIdx] => this[colIdx, rowIdx]!;
        object IImmutableTable.this[TablePointer.Pos pos] => this[pos]!;
        object IImmutableTable.this[TablePointer pointer] => this[pointer]!;
    }
}
