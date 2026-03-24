#nullable enable
using CCEnvs.FuncLanguage;

namespace CCEnvs.Unity.Items
{
    public interface IItemAccessor
    {
        Maybe<IItemContainerInfo> PutItem(IItem? item, int count = 1);
        Maybe<IItemContainerInfo> PutItemFrom(IItemContainer itemContainer, int count);
        Maybe<IItemContainerInfo> PutItemFrom(IItemContainer itemContainer);

        Maybe<IItemContainerInfo> TakeItem(int count);
        Maybe<IItemContainerInfo> TakeItem();
        Maybe<IItemContainerInfo> TakeItem(IItem item, int count);

        void CopyItemFrom(IItemContainerInfo itemContainer);

        void Clear();
    }
}
