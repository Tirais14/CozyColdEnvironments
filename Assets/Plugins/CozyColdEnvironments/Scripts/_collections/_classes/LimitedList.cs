using CCEnvs.Collections.ObjectModel;
using System;
using System.Collections.Generic;

#nullable enable
namespace CCEnvs.Collections
{
    [Obsolete("Cause issues", true)]
    public class LimitedList<T> : ListBase<T>, IReadOnlyList<T>, ICollection<T>
    {
        public static LimitedList<T> Empty { get; } = new();

        public LimitedList(int length)
            :
            base(length)
        {
        }

        public LimitedList(T[] array)
        {
            inner = array;

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].IsDefault())
                    pointer = i - 1;
            }
        }

        public LimitedList()
            :
            this(Array.Empty<T>())
        {
        }

        public override void Add(T item)
        {
            if (!HasEmptySlot)
                throw new CollectionOverflowedException();

            base.Add(item);
        }

        public override void Insert(int index, T item)
        {
            if (!HasEmptySlot)
                throw new CollectionOverflowedException();

            base.Insert(index, item);
        }
    }
}
