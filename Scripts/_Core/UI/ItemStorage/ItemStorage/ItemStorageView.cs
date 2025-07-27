using UTIRLib.GameSystems.Storage;
using UTIRLib.Reflection;
using UTIRLib.UI.MVVM;

#nullable enable
namespace UTIRLib.UI.ItemStorage
{
    public class ItemStorageView<TViewModel, TModel> : AViewLazy<TViewModel>
        where TViewModel : IViewModel<TModel>
        where TModel : IItemStorageReactive
    {
        protected override TViewModel CreateViewModel()
        {
            TModel model = CreateModel();

            var creationParams = new ConstructorParameters
            {
                BindingFlags = BindingFlagsDefault.InstancePublic,
                ArgumentsData = InvokableArguments.Create(model)
            };

            return InstanceFactory.Create<TViewModel>(creationParams,
                                                      cacheResults: true);
        }

        private TModel CreateModel()
        {
            IItemSlot[] slots = this.GetAssignedModelsInChildren<IItemSlot>();

            var creationParams = new ConstructorParameters
            {
                BindingFlags = BindingFlagsDefault.InstancePublic,
                ArgumentsData = InvokableArguments.Create(slots)
            };

            if (!InstanceFactory.TryCreate<TModel>(creationParams,
                                       cacheResult: true,
                                       out var model))
            {
                model = InstanceFactory.Create<TModel>(creationParams with
                {
                    ArgumentsData = InvokableArguments.Empty
                },
                cacheResults: true);
            }

            return model;
        }
    }
}
