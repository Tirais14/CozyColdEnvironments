using UniRx;
using UnityEngine;

#nullable enable

namespace UTIRLib.UI
{
    [RequireComponent(typeof(StorageModel))]
    public class StorageView : View<StorageViewModel>
    {
        protected void SetGameObjectState(bool state) => gameObject.SetActive(state);

        protected override void BindToViewModel()
        {
            viewModel.IsOpenedView.Subscribe(SetImageEnabledState).AddTo(this);
            viewModel.IsOpenedView.Subscribe(SetGameObjectState).AddTo(this);
        }
    }
}