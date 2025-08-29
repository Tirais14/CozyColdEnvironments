using System;
using System.Linq;
using UniRx;
using UnityEngine;
using CozyColdEnvironments.Diagnostics;
using CozyColdEnvironments.GameSystems.ItemStorageSystem;
using CozyColdEnvironments.Reflection;
using CozyColdEnvironments.Reflection.ObjectModel;
using CozyColdEnvironments.UI.MVVM;
using CozyColdEnvironments.Unity;
using CozyColdEnvironments.Unity.TypeMatching;

#nullable enable
namespace CozyColdEnvironments.UI.ItemStorageSystem
{
    public class ItemStorageView<TViewModel, TModel> : AView<TViewModel>
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
