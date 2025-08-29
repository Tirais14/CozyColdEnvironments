using UniRx;
using CozyColdEnvironments.GameSystems;
using CozyColdEnvironments.GameSystems.ItemStorageSystem;

#nullable enable
namespace CozyColdEnvironments.UI.ItemStorageSystem
{
    public interface IItemContainerReactive : IItemContainer
    {
        IReadOnlyReactiveProperty<IStorageItem> ItemReactive { get; }
        IReadOnlyReactiveProperty<int> ItemCountReactive { get; }
    }
    public interface IItemContainerReactive<TContainer> : IItemContainer<TContainer>
        where TContainer : IItemContainer
    {
    }
    public interface IItemContainerReactive<TContainer, TItem> : IItemContainer<TContainer, TItem>
        where TContainer : IItemContainer
        where TItem : IStorageItem, new()
    {
    }
}
