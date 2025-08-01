#nullable enable
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace UTIRLib.GameSystems.ItemStorageSystem
{
    public interface IItemStorage : IReadOnlyList<IItemSlot>
    {
        int SlotCount { get; }

        IItemSlot GetSlot(int id);

        void RemoveSlot(int id);

        IItemStack AddItem(IStorageItem item, int count);

        void AddItem(IItemStack itemStack);

        bool HasItem(IStorageItem item, int count = 1);

        int GetSlotId(IItemSlot slot);

        bool Contains(IItemStack itemStack);

        bool Contains(IItemSlot slot);

        bool CanHold(IStorageItem item);

        IItemSlot? GetEmptySlot();

        bool TryGetEmptySlot([NotNullWhen(true)] out IItemSlot? result);

        IItemSlot? GetSuitableSlot(IStorageItem item);

        bool TryGetSuitableSlot(IStorageItem item, [NotNullWhen(true)] out IItemSlot? result);
    }
    public interface IItemStorage<T> : IItemStorage
        where T : IItemSlot
    {
        new T this[int id] { get; }

        IItemSlot IReadOnlyList<IItemSlot>.this[int index] => this[index];

        new T GetSlot(int id);

        void AddItemSlot(T itemSlot);

        int GetSlotId(T slot);

        bool Contains(T slot);

        new T? GetEmptySlot();

        bool TryGetEmptySlot([NotNullWhen(true)] out T? result);

        new T? GetSuitableSlot(IStorageItem item);

        bool TryGetSuitableSlot(IStorageItem item, [NotNullWhen(true)] out T? result);

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

        IItemSlot? IItemStorage.GetSuitableSlot(IStorageItem item) => GetSuitableSlot(item);

        bool IItemStorage.TryGetSuitableSlot(IStorageItem item,
            [NotNullWhen(true)] out IItemSlot? result)
        {
            return TryGetSuitableSlot(item, out result);
        }
    }
}
