#nullable enable
using UniRx;
using UTIRLib.GameSystems.ItemStorageSystem;

namespace UTIRLib.UI.ItemStorageSystem
{
    public interface IItemStackReactive : IItemStack
    {
        IReadOnlyReactiveProperty<IStorageItem> ItemReactive { get; }
        IReadOnlyReactiveProperty<int> ItemCountReactive { get; }
    }
    public interface IItemStackReactive<T> : IItemStackReactive, IItemStack<T>
        where T : IStorageItem, new()
    {
    }
}
