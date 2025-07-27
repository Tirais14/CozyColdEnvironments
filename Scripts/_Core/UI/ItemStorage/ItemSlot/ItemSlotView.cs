using UTIRLib.Diagnostics;
using UTIRLib.GameSystems.Storage;
using UTIRLib.Reflection;
using UTIRLib.UI.MVVM;

#nullable enable
namespace UTIRLib.UI.ItemStorage
{
    public class ItemSlotView<TViewModel, TModel> : AViewLazy<TViewModel>
        where TViewModel : IViewModel<TModel>
        where TModel : IItemSlot
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
            IItemStack? itemStack = this.GetAssignedModelInChidlren<IItemStack>();

            if (itemStack.IsNull())
                throw new ObjectNotFoundException(typeof(IItemStack));

            var creationParams = new ConstructorParameters
            {
                BindingFlags = BindingFlagsDefault.InstancePublic,
                ArgumentsData = InvokableArguments.Create(itemStack)
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
