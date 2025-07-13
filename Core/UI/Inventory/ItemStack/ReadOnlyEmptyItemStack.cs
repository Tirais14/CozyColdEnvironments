using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace UTIRLib.UI
{
    /// <summary>
    /// Immutable empty item stack
    /// </summary>
    public class ReadOnlyEmptyItemStack : IItemStack
    {
        public IItem? Item => null;
        public int Quantity => 0;
        public bool IsEmpty => true;
        public bool IsFull => false;

        /// <returns><see langword="false"/></returns>
        public bool Contains(IItem item) => false;

        /// <returns><see langword="default"/></returns>
        public T? GetItem<T>() where T : IItem => default;

        /// <exception cref="NotSupportedException"></exception>
        public IItemStack Put(IItem item, int quantity = 1) => throw new NotSupportedException();

        /// <exception cref="NotSupportedException"></exception>
        public void Put(IItemStack itemStack) => throw new NotSupportedException();

        /// <returns>empty</returns>
        public IItemStack Take(int quantity = 1) => new ReadOnlyEmptyItemStack();

        /// <returns>empty</returns>
        public IItemStack TakeAll() => new ReadOnlyEmptyItemStack();

        /// <returns><see langword="false"/></returns>
        public bool TryGetItem<T>([NotNullWhen(true)] out T? item) where T : IItem
        {
            item = default;
            return false;
        }

        public void Clear()
        { }
    }
}