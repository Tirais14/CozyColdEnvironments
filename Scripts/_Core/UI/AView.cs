#nullable enable
using UniRx;
using UTIRLib.Attributes;

namespace UTIRLib.UI
{
    public abstract class AView<T> : MonoX, IView<T>
        where T : IViewModel
    {
        [RequiredField]
        protected T viewModel;

        protected override void OnAwake()
        {
            base.OnAwake();

            onEndFirstFrame += () => viewModel.AddTo(this);
        }

        public T GetViewModel() => viewModel;
    }
}
