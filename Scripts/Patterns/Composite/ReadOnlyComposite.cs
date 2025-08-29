using System;
using System.Collections;
using System.Collections.Generic;
using CozyColdEnvironments.Collections;
using CozyColdEnvironments.Diagnostics;
using CozyColdEnvironments.Extensions;
using CozyColdEnvironments.Linq;

#nullable enable

namespace CozyColdEnvironments.Patterns.Composite
{
    public class ReadOnlyComposite<T> : IReadOnlyComposite<T>
    {
        protected readonly T[] childs = null!;
        protected int childsCount;

        public int Count => childsCount;
        public T this[int index] => childs[index];

        public ReadOnlyComposite()
        { }

        public ReadOnlyComposite(T[] childs)
        {
            this.childs = childs;
            childsCount = childs.Length;
        }

        public T GetChild(int index) => childs[index];

        public bool Contains(object? value) => Contains(value.IsQ<T>());

        public virtual bool Contains(T? value)
        {
            return value.IsNotDefault() && Array.IndexOf(childs, value) >= 0;
        }

        public IEnumerator<T> GetEnumerator() => (IEnumerator<T>)childs.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => childs.GetEnumerator();

        object IReadOnlyComposite.GetChild(int index) => GetChild(index)!;
    }
}