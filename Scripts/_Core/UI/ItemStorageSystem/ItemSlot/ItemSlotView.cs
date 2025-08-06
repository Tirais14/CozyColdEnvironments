using UniRx;
using UnityEngine.EventSystems;
using UTIRLib.Diagnostics;
using UTIRLib.GameSystems.ItemStorageSystem;
using UTIRLib.Reflection;
using UTIRLib.UI.MVVM;
using UTIRLib.Unity.TypeMatching;

#nullable enable
namespace UTIRLib.UI.ItemStorageSystem
{
    public class ItemSlotView<TViewModel, TModel> : AView<TViewModel>,
        IDropHandler
        where TViewModel : IItemSlotViewModel<TModel>
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
            if (!this.GetAssignedModelInChildren<IItemStack>()
                     .Is<IItemStack>(out var itemStack)
                     )
                throw new ObjectNotFoundException(typeof(IItemStack));

            return InstanceFactory.Create<TModel>(InvokableArguments.Create(itemStack,
                    InvokableArguments.CreationSettings.AllowSignatureTypesInheritance),
                    cacheConstructor: true);
        }

        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            viewModel.OnViewDrop(eventData);
        }
    }
}
