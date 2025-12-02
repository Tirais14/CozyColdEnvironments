#nullable enable
using System.Collections.Generic;

namespace CCEnvs.Collections
{
    public interface ITable : IImmutableTable
    {
        new object this[int colIdx, int rowIdx] { get; set; }
        new object this[TablePointer.Pos pos] { get; set; }
        new object this[TablePointer pointer] { get; set; }

        object IImmutableTable.this[int colIdx, int rowIdx] => this[colIdx, rowIdx];
        object IImmutableTable.this[TablePointer.Pos pos] => this[pos]!;
        object IImmutableTable.this[TablePointer pointer] => this[pointer]!;

        //void Add(int colIdx, int rowIdx, object value);

        bool Remove(object value);

        void RemoveAt(int colIdx, int rowIdx);

        void Clear();
    }
    public interface ITable<T> : ITable, IImmutableTable<T>
    {
        object ITable.this[int colIdx, int rowIdx] {
            get => this[colIdx, rowIdx]!;
            set => this[colIdx, rowIdx] = value.To<T>();
        }
        object ITable.this[TablePointer.Pos pos] {
            get => this[pos]!;
            set => this[pos] = value.To<T>();
        }
        object ITable.this[TablePointer pointer] {
            get => this[pointer]!;
            set => this[pointer] = value.To<T>();
        }

        new T this[int colIdx, int rowIdx] { get; set; }
        new T this[TablePointer.Pos pos] { get; set; }
        new T this[TablePointer pointer] { get; set; }

        object IImmutableTable.this[int colIdx, int rowIdx] => this[colIdx, rowIdx]!;
        object IImmutableTable.this[TablePointer.Pos pos] => this[pos]!;
        object IImmutableTable.this[TablePointer pointer] => this[pointer]!;

        T IImmutableTable<T>.this[int colIdx, int rowIdx] => this[colIdx, rowIdx];
        T IImmutableTable<T>.this[TablePointer.Pos pos] => this[pos]!;
        T IImmutableTable<T>.this[TablePointer pointer] => this[pointer]!;

        //void Add(int colIdx, int rowIdx, T value);

        bool Remove(T value);

        //void ITable.Add(int colIdx, int rowIdx, object value)
        //{
        //    Add(colIdx, rowIdx, value.As<T>());
        //}

        bool ITable.Remove(object value)
        {
            if (value is not T typed)
                return false;

            return Remove(typed);
        }

        IEnumerable<(int col, int row, T value)> AsIndexedEnumerable();

        IEnumerable<(TablePointer.Pos pos, T value)> AsPositionedEnumerable();
    }
}
