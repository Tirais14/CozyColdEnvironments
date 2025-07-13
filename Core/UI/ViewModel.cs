#nullable enable

using UnityEngine;
using UTIRLib.Injector;

namespace UTIRLib.UI
{
    public abstract class ViewModel : MonoX
    {
        protected override void OnStart()
        {
            base.OnStart();
            BindToModel();
        }

        protected abstract void BindToModel();
    }

    public abstract class ViewModel<TModel> : ViewModel
        where TModel : class
    {
        [SerializeField]
        [GetComponentIfNull]
        protected TModel model = null!;
    }
}