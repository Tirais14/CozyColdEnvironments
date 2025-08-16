using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UTIRLib.Attributes;
using UTIRLib.GameSystems.ItemStorageSystem;
using UTIRLib.Reflection;
using UTIRLib.UI.MVVM;

#nullable enable
namespace UTIRLib.UI.ItemStorageSystem
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

            TViewModel viewModel = InstanceFactory.Create<TViewModel>(
                InvokableArguments.Create(model,
                    InvokableArguments.CreationSettings.AllowSignatureTypesInheritance),
                 parameters: InstanceCreationParameters.CacheConstructor);

            viewModel.AddTo(this);

            return viewModel;
        }

        private static TModel CreateModel()
        {
            return InstanceFactory.Create<TModel>(InvokableArguments.Create(int.MaxValue,
                    InvokableArguments.CreationSettings.AllowSignatureTypesInheritance),
                parameters: InstanceCreationParameters.CacheConstructor);
        }
    }
}
