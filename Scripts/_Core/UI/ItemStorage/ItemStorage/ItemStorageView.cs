using System;
using System.Linq;
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
                InvokableArguments.Create(model,
                    InvokableArguments.CreationSettings.AllowSignatureTypesInheritance),
                cacheConstructor: true);

            viewModel.AddTo(this);

            return viewModel;
        }

        private TModel CreateModel()
        {
            Type[] modelGenericArguments = TypeHelper.CollectGenericArgumentsFromBaseClasses(typeof(TModel));

            if (modelGenericArguments.IsEmpty())
                throw new Exception("Not found generic arguments.");

            IItemSlot[] slotsUntyped = this.GetAssignedModelsInChildren<IItemSlot>();

            InvokableArguments creationArguments;
            if (slotsUntyped.IsNotEmpty())
                creationArguments = InvokableArguments.Create(slotsUntyped,
                    InvokableArguments.CreationSettings.CastArraysToElementType 
                    | 
                    InvokableArguments.CreationSettings.AllowSignatureTypesInheritance);
            else
            {
                Type? slotType = modelGenericArguments.First(x => x.IsType<IItemSlot>());
                Type slotsArrayType = slotType.MakeArrayType();

                Array slotsTyped = Array.CreateInstance(slotsArrayType, 0);

                creationArguments = InvokableArguments.Create(slotsTyped,
                    InvokableArguments.CreationSettings.CastArraysToElementType
                    |
                    InvokableArguments.CreationSettings.AllowSignatureTypesInheritance);
            }

            if (!InstanceFactory.TryCreate<TModel>(creationArguments,
                                                   out var model,
                                                   cacheConstructor: true)
                ) 
                model = InstanceFactory.Create<TModel>(InvokableArguments.Empty,
                                                       cacheConstructor: true);

            return model;
        }
    }
}
