#nullable enable
using CCEnvs.FuncLanguage;

namespace CCEnvs.Unity.Storages
{
    public interface IItemAccessor
    {
        Maybe<IItemContainer> PutItem(IItem? item, int count = 1);
        Maybe<IItemContainer> PutItemFrom(IItemContainer itemContainer, int count);
        Maybe<IItemContainer> PutItemFrom(IItemContainer itemContainer);

        Maybe<IItemContainer> TakeItem(int count);
        Maybe<IItemContainer> TakeItem();
        Maybe<IItemContainer> TakeItem(IItem item, int count);

        void CopyFrom(IItemContainerInfo itemContainer);

        void Clear();
    }
}
