using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using UniRx;

#nullable enable
#pragma warning disable S1117
#pragma warning disable IDE1006 
namespace UTIRLib.UI
{
    public abstract class AViewLazy<T> : MonoX, IView
        where T : IViewModel
    {
        private LazyX<T> viewModelLazy = null!;

        protected T viewModel => viewModelLazy.Value;

        protected override void OnAwake()
        {
            base.OnAwake();

            viewModelLazy = new LazyX<T>(CreateViewModel);
            _ = BindViewModel();
        }

        protected abstract T CreateViewModel();

        private async UniTaskVoid BindViewModel()
        {
            await UniTask.WaitUntil(() => viewModel is not null);

            viewModel.AddTo(this);
        }

        public IViewModel GetViewModel() => viewModel;
    }
}
