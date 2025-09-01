using CCEnvs.Reflection;
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
        public ComponentCache Cache { get; private set; } = null!;

        protected override void OnAwake()
        {
            base.OnAwake();

            Cache = this.GetCache();
        }

        protected override void OnStart()
        {
            base.OnStart();

            viewModel.IsOpenedView.Subscribe(x => Cache.gameObject.SetActive(x)).AddTo(this);
        }

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
            IItemSlot[] slots = this.GetAssignedModelsInChildren<IItemSlot>();

            return InstanceFactory.Create<TModel>(new ConstructorBindings
            {
                BindingFlags = BindingFlagsDefault.InstanceAll,
                Arguments = new ExplicitArguments(new TypeValuePair(typeof(IItemSlot[]), slots))
            }, InstanceFactory.Parameters.Default
               |
               InstanceFactory.Parameters.CacheConstructor);
        }
    }
}
