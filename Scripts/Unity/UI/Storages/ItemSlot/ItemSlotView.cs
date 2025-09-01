using CCEnvs.Reflection;
using CCEnvs.Reflection.Data;
using CCEnvs.UI.MVVM;
using CCEnvs.Unity.Diagnostics;
using CCEnvs.Unity.GameSystems.Storages;
using CCEnvs.Unity.TypeMatching;
using CCEnvs.Unity.UI.MVVM;
using UniRx;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
{
    public class ItemSlotView<TViewModel, TModel> 
        :
        AView<TViewModel>

        where TViewModel : IItemSlotViewModel<TModel>
        where TModel : IItemSlot
    {
        protected override TViewModel CreateViewModel()
        {
            TModel model = CreateModel();

            TViewModel viewModel = InstanceFactory.Create<TViewModel>(new ConstructorBindings
            {
                BindingFlags = BindingFlagsDefault.InstanceAll,
                Arguments = new ExplicitArguments(new TypeValuePair(model))
            }, InstanceFactory.Parameters.Default
               |
               InstanceFactory.Parameters.CacheConstructor);

            viewModel.AddTo(this);

            return viewModel;
        }

        private TModel CreateModel()
        {
            if (!this.GetAssignedModelInChildren<IItemStack>()
                     .Is<IItemStack>(out var itemStack)
                     )
                throw new ObjectNotFoundException(typeof(IItemStack));

            return InstanceFactory.Create<TModel>(new ConstructorBindings
            {
                BindingFlags = BindingFlagsDefault.InstanceAll,
                Arguments = new ExplicitArguments(
                    new TypeValuePair(typeof(IItemStack), itemStack))
            }, InstanceFactory.Parameters.Default
               |
               InstanceFactory.Parameters.CacheConstructor);
        }
    }
}
