using NUnit.Framework.Constraints;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using CCEnvs.Attributes;
using CCEnvs.GameSystems.ItemStorageSystem;
using CCEnvs.Reflection;
using CCEnvs.Reflection.ObjectModel;
using CCEnvs.UI.MVVM;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
{
    [RequireComponent(typeof(Image))]
    public class ItemStackView<TViewModel, TModel>  : AView<TViewModel>
        where TViewModel : IItemStackViewModel<TModel>
        where TModel : IItemStack, IItemContainerReactive
    {
        [GetBySelf]
        protected Image image;

        [Optional]
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

            TViewModel viewModel = InstanceFactory.Create<TViewModel>(new ConstructorBindings
            {
                BindingFlags = BindingFlagsDefault.InstanceAll,
                Arguments = new ExplicitArguments(new TypeValuePair(model))
            });

            viewModel.AddTo(this);

            return viewModel;
        }

        private static TModel CreateModel()
        {
            return InstanceFactory.Create<TModel>(new ConstructorBindings
            {
                BindingFlags = BindingFlagsDefault.InstanceAll,
                Arguments = new ExplicitArguments(new TypeValuePair(int.MaxValue))
            });
        }
    }
}
