using System.Xml;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UTIRLib.GameSystems.Storage;
using UTIRLib.Reflection;
using UTIRLib.UI.MVVM;
using UTIRLib.Unity.TypeMatching;

#nullable enable
namespace UTIRLib.UI.ItemStorage
{
    public class ItemStackView<TViewModel, TModel>  : AView<TViewModel>,
        IDropHandler
        where TViewModel : IItemStackViewModel<TModel>
        where TModel : IItemStackReactive
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

        private static TModel CreateModel()
        {
            if (!InstanceFactory.TryCreate<TModel>(InvokableArguments.Create(int.MaxValue,
                    InvokableArguments.CreationSettings.AllowSignatureTypesInheritance),
                                                   out var model,
                                                   cacheConstructor: true)
                )
                model = InstanceFactory.Create<TModel>(InvokableArguments.Empty,
                                                       cacheConstructor: true);

            return model;
        }

        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            viewModel.OnViewDrop(eventData);
        }
    }
}
