using UniRx;
using UnityEngine;

#nullable enable

namespace UTIRLib.UI
{
    [RequireComponent(typeof(StorageModel))]
    public class StorageViewModel : ViewModel<StorageModel>
    {
        public IReadOnlyReactiveProperty<bool> IsOpenedView => model.IsOpenedProp;

        protected override void BindToModel()
        { }
    }
}