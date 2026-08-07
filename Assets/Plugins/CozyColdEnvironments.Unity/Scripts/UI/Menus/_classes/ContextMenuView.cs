using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.UI.Menus
{
    public abstract class ContextMenuView<TViewModel> : View<TViewModel>
        where TViewModel : IContextMenuViewModel
    {
        [SerializeField]
        protected Transform itemsRoot = null!;

        public Transform ItemsRoot {
            get => itemsRoot;
            set => SetItemsRoot(value);
        }

        protected override void Start()
        {
            base.Start();
            SetItemsRoot(itemsRoot);
        }

        protected override void OnSetViewModel(TViewModel? viewModel) { }

        protected override void InitViewModel(TViewModel viewModel) { }

        public ContextMenuView<TViewModel> SetItemsRoot(Transform? value)
        {
            itemsRoot = value.IfNull(transform);
            return this;
        }
    }
}
