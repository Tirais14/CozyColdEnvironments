using CCEnvs.Attributes;
using CCEnvs.Reflection.Data;
using CCEnvs.UI;
using CCEnvs.Unity.ComponentSetter;
using CCEnvs.Unity.GameSystems.Storages;
using CCEnvs.Unity.UI.MVVM;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
{
    [RequireComponent(typeof(Image))]
    public class ItemStackView<TViewModel, TModel> 
        :
        AView<TViewModel>

        where TViewModel : IItemStackViewModel<TModel>
        where TModel : IItemStack, IItemContainerReactive
    {
        [GetBySelf]
        protected Image image;

        [OptionalField]
        [GetByChildren]
        [SerializeField]
        protected ATextView? textView;

        protected override void OnStart()
        {
            base.OnStart();

            if (textView != null)
                viewModel.CounterView.Subscribe(x => textView.Text = x).AddTo(this);

            viewModel.IconView.Subscribe(x => image.sprite = x).AddTo(this);
        }

        protected override TViewModel CreateViewModel()
        {
            TModel model = CreateModel();

            TViewModel viewModel = InstanceFactory.Create<TViewModel>(
                new ExplicitArguments(new ExplicitArgument(model)),
                InstanceFactory.Parameters.ThrowIfNotFound
                |
                InstanceFactory.Parameters.CacheConstructor
                |
                InstanceFactory.Parameters.NonPublic);

            viewModel.AddTo(this);

            return viewModel;
        }

        private static TModel CreateModel()
        {
            return InstanceFactory.Create<TModel>(
                new ExplicitArguments(new ExplicitArgument(int.MaxValue)),
                InstanceFactory.Parameters.ThrowIfNotFound
                |
                InstanceFactory.Parameters.CacheConstructor
                |
                InstanceFactory.Parameters.NonPublic);
        }
    }
}
