#nullable enable
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace UTIRLib.GameSystems.Storage
{
    public interface IItemStorage
    {
        int SlotCount { get; }

        IItemSlot this[int id] { get; }

        IItemSlot GetSlot(int id);

        void RemoveSlot(int id);

        IItemStack AddItem(IItem item, int count);

        void AddItem(IItemStack itemStack);

        bool HasItem(IItem item, int count = 1);

        int GetSlotId(IItemSlot slot);

        bool Contains(IItemStack itemStack);

        bool Contains(IItemSlot slot);

        bool CanHold(IItem item);

        IItemSlot? GetEmptySlot();

        bool TryGetEmptySlot([NotNullWhen(true)] out IItemSlot? result);

        IItemSlot? GetSuitableSlot(IItem item);

        bool TryGetSuitableSlot(IItem item, [NotNullWhen(true)] out IItemSlot? result);
    }
    public interface IItemStorage<T> : IItemStorage, IReadOnlyList<T>
        where T : IItemSlot
    {
        new T this[int id] { get; }

        IItemSlot IItemStorage.this[int id] => this[id];
        int IReadOnlyCollection<T>.Count => SlotCount;

        new T GetSlot(int id);

        void AddItemSlot(T itemSlot);

        int GetSlotId(T slot);

        bool Contains(T slot);

        new T? GetEmptySlot();

        bool TryGetEmptySlot([NotNullWhen(true)] out T? result);

        new T? GetSuitableSlot(IItem item);

        bool TryGetSuitableSlot(IItem item, [NotNullWhen(true)] out T? result);

        IItemSlot IItemStorage.GetSlot(int id) => GetSlot(id);

        IItemSlot? IItemStorage.GetEmptySlot() => GetEmptySlot();

        int IItemStorage.GetSlotId(IItemSlot slot)
        {
            if (slot is not T typed)
                return int.MinValue;

            return GetSlotId(typed);
        }

        bool IItemStorage.Contains(IItemSlot slot)
        {
            return slot is T typed && Contains(typed);
        }

        bool IItemStorage.TryGetEmptySlot([NotNullWhen(true)] out IItemSlot? result)
        {
            return TryGetEmptySlot(out result);
        }

        IItemSlot? IItemStorage.GetSuitableSlot(IItem item) => GetSuitableSlot(item);

        bool IItemStorage.TryGetSuitableSlot(IItem item,
            [NotNullWhen(true)] out IItemSlot? result)
        {
            return TryGetSuitableSlot(item, out result);
        }
    }
}
