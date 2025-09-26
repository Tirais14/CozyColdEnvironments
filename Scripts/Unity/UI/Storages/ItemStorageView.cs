using CCEnvs.Reflection.Data;
using CCEnvs.UI.MVVM;
using CCEnvs.Unity.GameSystems.Storages;
using CCEnvs.Unity.UI.MVVM;
using UniRx;

#nullable enable
namespace CCEnvs.Unity.UI.Storages
{
    public class ItemStorageView<TViewModel, TModel>
        :
        AView<TViewModel>

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
                new ExplicitArguments(new ExplicitArgument(model)),
                InstanceFactory.Parameters.Default 
                | 
                InstanceFactory.Parameters.CacheConstructor
                |
                InstanceFactory.Parameters.NonPublic);

            viewModel.AddTo(this);

            return viewModel;
        }

        private TModel CreateModel()
        {
            IItemSlot[] slots = this.GetAssignedModelsInChildren<IItemSlot>();

            return InstanceFactory.Create<TModel>(
                new ExplicitArguments(ExplicitArgument.T(slots)),
                InstanceFactory.Parameters.Default
                |
                InstanceFactory.Parameters.CacheConstructor
                |
                InstanceFactory.Parameters.NonPublic);
        }
    }
}
