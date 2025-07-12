using UniRx;
using UnityEngine;
using UTIRLib.Injector;

#nullable enable

namespace UTIRLib.UI
{
    public class DragItemStack : MonoX, IItemStackProvider
    {
        [SerializeField]
        [GetComponentInParentIfNull]
        protected ItemStackModelBase sourceItemStack = null!;

        protected readonly ReactiveProperty<bool> isActive = new();

        public IReadOnlyReactiveProperty<bool> IsActive => isActive;
        public IItemStack ItemStack => sourceItemStack;

        IReadOnlyItemStack IReadOnlyItemStackProvider.ItemStack => ItemStack;

        private void OnEnable() => isActive.Value = true;

        private void OnDisable() => isActive.Value = false;
    }
}