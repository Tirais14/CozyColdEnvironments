using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UTIRLib.GameSystems.ItemStorageSystem;
using UTIRLib.Reflection;
using UTIRLib.UI.MVVM;
using UTIRLib.Unity;

#nullable enable
namespace UTIRLib.UI.ItemStorageSystem
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

            TViewModel viewModel = InstanceFactory.Create<TViewModel>(
                InvokableArguments.Create(model,
                    InvokableArguments.CreationSettings.AllowSignatureTypesInheritance),
                cacheConstructor: true);

            viewModel.AddTo(this);

            return viewModel;
        }

        private TModel CreateModel()
        {
            IItemSlot[] slotsUntyped = this.GetAssignedModelsInChildren<IItemSlot>();

            InvokableArguments creationArguments;
            if (slotsUntyped.IsNotEmpty())
                creationArguments = InvokableArguments.Create(slotsUntyped,
                    InvokableArguments.CreationSettings.CastArraysToElementType 
                    | 
                    InvokableArguments.CreationSettings.AllowSignatureTypesInheritance);
            else
            {
                Type[] modelGenericArguments = TypeHelper.CollectGenericArgumentsFromBaseClasses(typeof(TModel));

                if (modelGenericArguments.IsEmpty())
                {
                    creationArguments = InvokableArguments.Create(typeof(IItemSlot[]),
                        InvokableArguments.CreationSettings.CastArraysToElementType
                        |
                        InvokableArguments.CreationSettings.AllowSignatureTypesInheritance);

                    if (!InstanceFactory.TryCreate<TModel>(creationArguments, out var model))
                        throw new Exception("Generic arguments not found.");

                    return model;
                }

                Type? slotType = modelGenericArguments.First(x => x.IsType<IItemSlot>());
                Type slotsArrayType = slotType.MakeArrayType();

                Array slotsTyped = Array.CreateInstance(slotsArrayType, 0);

                creationArguments = InvokableArguments.Create(slotsTyped,
                    InvokableArguments.CreationSettings.CastArraysToElementType
                    |
                    InvokableArguments.CreationSettings.AllowSignatureTypesInheritance);
            }

            return InstanceFactory.Create<TModel>(creationArguments,
                                                  cacheConstructor: true);
        }
    }
}
