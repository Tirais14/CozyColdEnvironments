using CCEnvs.FuncLanguage;
using R3;

#nullable enable
namespace CCEnvs.UnityX.Items
{
    public interface IItemContainerInfo
        :
        IItemContainerInfoItemless
    {
        Maybe<IItem> Item { get; }

        Observable<Maybe<IItem>> ObserveItem();
    }

    public interface IItemContainerInfo<TItem> : IItemContainerInfo
        where TItem : IItem
    {
        new Maybe<TItem> Item { get; }

        Maybe<IItem> IItemContainerInfo.Item => ((IItem?)Item.GetValue()).Maybe();

        new Observable<Maybe<TItem>> ObserveItem();

        Observable<Maybe<IItem>> IItemContainerInfo.ObserveItem()
        {
            return ObserveItem().Select(item => ((IItem?)item.GetValue()).Maybe());
        }
    }
}
