using CCEnvs.Unity.GameSystems.Storages;
using UniRx;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
{
    public class ItemStorageReactive
        :
        ItemStorage, 
        IItemStorageReactive
    {
        private readonly ReactiveProperty<bool> isOpenedReactive = new();

        public bool IsOpened => isOpenedReactive.Value;

        public IReadOnlyReactiveProperty<bool> IsOpenedReactive => isOpenedReactive;

        public ItemStorageReactive(IItemSlot[] slots) : base(slots)
        {
        }

        public void Close() => isOpenedReactive.Value = false;

        public void Open() => isOpenedReactive.Value = true;

        public bool SwitchOpenableState() => isOpenedReactive.Value = !IsOpened;
    }

    public class ItemStorageReactive<T> : ItemStorage<T>, IItemStorageReactive<T>
        where T : IItemSlot
    {
        private readonly ReactiveProperty<bool> isOpenedReactive = new();

        public bool IsOpened => isOpenedReactive.Value;

        public IReadOnlyReactiveProperty<bool> IsOpenedReactive => isOpenedReactive;

        public ItemStorageReactive(T[] slots) : base(slots)
        {
        }

        public void Close() => isOpenedReactive.Value = false;

        public void Open() => isOpenedReactive.Value = true;

        public bool SwitchOpenableState() => isOpenedReactive.Value = !IsOpened;
    }
}
