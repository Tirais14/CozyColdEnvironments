#nullable enable
using System.Diagnostics.CodeAnalysis;

namespace UTIRLib.GameSystems.Storage
{
    public interface IItemStorage
    {
        int SlotCount { get; }

        IItemSlot this[int index] { get; }

        IItemSlot GetItemSlot(int index);

        void RemoveItemSlotAt(int index);

        void AddItem(IItem item, int count);

        void AddItem(IItemStack itemStack);

        bool HasItem(IItem item, int count = 1);

        bool HasItemStack(IItemStack itemStack);

        IItemSlot? GetEmptySlot();

        bool TryGetEmptySlot([NotNullWhen(true)] out IItemSlot? result);

        IItemSlot? GetSuitableSlot(IItem item);

        bool TryGetSuitableSlot(IItem item, [NotNullWhen(true)] out IItemSlot? result);
    }
    public interface IItemStorage<T> : IItemStorage
        where T : IItemSlot
    {
        new T this[int index] { get; }

        IItemSlot IItemStorage.this[int index] => this[index];

        new T GetItemSlot(int index);

        void AddItemSlot(T itemSlot);

        new T? GetEmptySlot();

        bool TryGetEmptySlot([NotNullWhen(true)] out T? result);

        new T? GetSuitableSlot(IItem item);

        bool TryGetSuitableSlot(IItem item, [NotNullWhen(true)] out T? result);

        IItemSlot IItemStorage.GetItemSlot(int index) => GetItemSlot(index);

        IItemSlot? IItemStorage.GetEmptySlot() => GetEmptySlot();

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
