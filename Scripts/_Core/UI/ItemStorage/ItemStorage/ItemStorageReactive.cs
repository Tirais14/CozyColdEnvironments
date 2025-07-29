using UniRx;
using UTIRLib.GameSystems.Storage;

#nullable enable
namespace UTIRLib.UI.ItemStorage
{
    public class ItemStorageReactive : GameSystems.Storage.ItemStorage, IItemStorageReactive
    {
        private readonly ReactiveProperty<bool> isOpenedReactive = new();

        public bool IsOpened => isOpenedReactive.Value;
        public IReadOnlyReactiveProperty<bool> IsOpenedReactive => isOpenedReactive;

        public ItemStorageReactive(IItemSlot[] slots) : base(slots)
        {
        }

        public void Close() => isOpenedReactive.Value = false;

        public void Open() => isOpenedReactive.Value = true;

        public bool SwitchOpenableState()
        {
            isOpenedReactive.Value = !IsOpened;

            return IsOpened;
        }
    }
}
