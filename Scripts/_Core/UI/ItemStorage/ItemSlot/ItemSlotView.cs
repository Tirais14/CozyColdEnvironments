using UniRx;
using UTIRLib.Diagnostics;
using UTIRLib.GameSystems.Storage;
using UTIRLib.Reflection;
using UTIRLib.UI.MVVM;
using UTIRLib.Unity.TypeMatching;

#nullable enable
namespace UTIRLib.UI.ItemStorage
{
    public class ItemSlotView<TViewModel, TModel> : AView<TViewModel>
        where TViewModel : IViewModel<TModel>
        where TModel : IItemSlot
    {
        protected override TViewModel CreateViewModel()
        {
            TModel model = CreateModel();

            TViewModel viewModel = InstanceFactory.Create<TViewModel>(
                InvokableArguments.Create(model,
                    InvokableArguments.CreationSettings.AllowSignatureTypesInheritance),
                cacheConstructor: true);

            viewModel.AddTo(this);

            return viewModel;
        }

        private TModel CreateModel()
        {
            if (!this.GetAssignedModelInChidlren<IItemStack>()
                     .Is<IItemStack>(out var itemStack)
                     )
                throw new ObjectNotFoundException(typeof(IItemStack));

            if (!InstanceFactory.TryCreate<TModel>(InvokableArguments.Create(itemStack,
                    InvokableArguments.CreationSettings.AllowSignatureTypesInheritance),
                                                  out var model,
                                                  cacheConstructor: true)
                )
                model = InstanceFactory.Create<TModel>(InvokableArguments.Empty,
                                                       cacheConstructor: true);

            return model;
        }
    }
}
