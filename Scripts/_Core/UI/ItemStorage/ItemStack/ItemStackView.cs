using UniRx;
using UTIRLib.Reflection;
using UTIRLib.UI.MVVM;

#nullable enable
namespace UTIRLib.UI.ItemStorage
{
    public class ItemStackView<TViewModel, TModel>  : AView<TViewModel>
        where TViewModel : IViewModel<TModel>
        where TModel : IItemStackReactive
    {
        protected override TViewModel CreateViewModel()
        {
            TModel model = CreateModel();

            TViewModel viewModel = InstanceFactory.Create<TViewModel>(
                InvokableArguments.Create(model),
                cacheResults: true);

            viewModel.AddTo(this);

            return viewModel;
        }

        private static TModel CreateModel()
        {
            if (!InstanceFactory.TryCreate<TModel>(InvokableArguments.Create(int.MaxValue),
                                                   out var model,
                                                   cacheResult: true)
                )
                model = InstanceFactory.Create<TModel>(InvokableArguments.Empty,
                                                       cacheResults: true);

            return model;
        }
    }
}
