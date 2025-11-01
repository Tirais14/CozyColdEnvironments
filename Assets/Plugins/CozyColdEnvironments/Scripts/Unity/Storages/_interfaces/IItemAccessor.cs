#nullable enable
using CCEnvs.FuncLanguage;

namespace CCEnvs.Unity.Storages
{
    public interface IItemAccessor
    {
        Maybe<IItemContainer> Put(IItem? item, int count);
        Maybe<IItemContainer> Put(IItemContainer itemContainer, int count);
        Maybe<IItemContainer> Put(IItemContainer itemContainer);

        Maybe<IItemContainer> Take(int count);
        Maybe<IItemContainer> Take();
        Maybe<IItemContainer> Take(IItem item, int count);

        void CopyFrom(IItemContainerInfo itemContainer);

        void Clear();
    }
}
