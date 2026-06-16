using R3;

#nullable enable
namespace CCEnvs.UnityX.Items
{
    public interface IItemContainerInfo
        :
        IItemContainerInfoItemless
    {
        IItem? Item { get; }

        Observable<IItem?> ObserveItem();
    }

    public interface IItemContainerInfo<TItem> : IItemContainerInfo
        where TItem : IItem
    {
        new TItem? Item { get; }

        IItem? IItemContainerInfo.Item => Item;

        new Observable<TItem?> ObserveItem();

        Observable<IItem?> IItemContainerInfo.ObserveItem()
        {
            return ObserveItem().Select(item => (IItem?)item);
        }
    }
}
