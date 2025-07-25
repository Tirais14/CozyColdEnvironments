using UniRx;

#nullable enable
namespace UTIRLib.UI.StorageSystem
{
    public interface IItemStackUIReactive : IItemStackUI
    {
        new IReadOnlyReactiveProperty<IItemUI> Item { get; }
        new IReadOnlyReactiveProperty<int> ItemCount { get; }
    }
}
