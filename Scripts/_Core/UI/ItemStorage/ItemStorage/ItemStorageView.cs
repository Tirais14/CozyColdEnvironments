using UniRx;
using UTIRLib.GameSystems.Storage;
using UTIRLib.Reflection;
using UTIRLib.UI.MVVM;

#nullable enable
namespace UTIRLib.UI.ItemStorage
{
    public class ItemStorageView<TViewModel, TModel> : AView<TViewModel>
        where TViewModel : IItemStorageViewModel<TModel>
        where TModel : IItemStorageReactive
    {
        protected override void OnStart()
        {
            base.OnStart();

            viewModel.IsOpenedView.Subscribe(x => gameObject.SetActive(x)).AddTo(this);
        }

        protected override TViewModel CreateViewModel()
        {
            TModel model = CreateModel();

            TViewModel viewModel = InstanceFactory.Create<TViewModel>(
                InvokableArguments.Create(model),
                cacheResults: true);

            viewModel.AddTo(this);

            return viewModel;
        }

        private TModel CreateModel()
        {
            IItemSlot[] slots = this.GetAssignedModelsInChildren<IItemSlot>();

            if (!InstanceFactory.TryCreate<TModel>(InvokableArguments.Create(slots),
                                                   out var model,
                                                   cacheResult: true)
                ) 
                model = InstanceFactory.Create<TModel>(InvokableArguments.Empty,
                                                       cacheResults: true);

            return model;
        }
    }
}
