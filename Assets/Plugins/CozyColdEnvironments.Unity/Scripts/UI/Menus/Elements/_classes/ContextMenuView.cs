using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.UI.Menus.Elements
{
    public abstract class ContextMenuView<TViewModel> : View<TViewModel>
        where TViewModel : IContextMenuViewModel
    {
        protected override void OnSetViewModel(TViewModel? viewModel) { }

        protected override void InitViewModel(TViewModel viewModel) { }
    }
}
