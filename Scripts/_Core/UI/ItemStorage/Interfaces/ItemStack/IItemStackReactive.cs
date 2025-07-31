#nullable enable
using UniRx;
using UTIRLib.GameSystems.Storage;

namespace UTIRLib.UI.ItemStorage
{
    public interface IItemStackReactive : IItemStack
    {
        IReadOnlyReactiveProperty<IItem> ItemReactive { get; }
        IReadOnlyReactiveProperty<int> ItemCountReactive { get; }
    }
    public interface IItemStackReactive<T> : IItemStackReactive, IItemStack<T>
        where T : IItem
    {
    }
}
