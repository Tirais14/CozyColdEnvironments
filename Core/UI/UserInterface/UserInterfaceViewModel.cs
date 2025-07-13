using UniRx;
using UnityEngine;

namespace UTIRLib.UI
{
    [RequireComponent(typeof(UserInterface))]
    public class UserInterfaceViewModel : ViewModel<UserInterface>
    {
        public IReadOnlyReactiveProperty<bool> IsOpenedView => model.IsOpenedProp;

        protected override void BindToModel()
        { }
    }
}