using UniRx;
using UnityEngine;

#nullable enable

namespace UTIRLib.UI
{
    [RequireComponent(typeof(UserInterfaceViewModel))]
    public class UserInterfaceView : View<UserInterfaceViewModel>
    {
        protected override void BindToViewModel()
        {
            viewModel.IsOpenedView.Subscribe(SetCanvasEnabledState).AddTo(this);
        }
    }
}