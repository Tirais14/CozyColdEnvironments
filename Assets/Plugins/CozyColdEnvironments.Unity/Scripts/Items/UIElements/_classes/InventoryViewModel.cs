#nullable enable
using CCEnvs.Collections;
using CCEnvs.Disposables;
using CCEnvs.Threading.Tasks;
using CCEnvs.UnityX.UI.Elements;
using Cysharp.Threading.Tasks;
using Humanizer;
using ObservableCollections;
using R3;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace CCEnvs.UnityX.Items.UIElements
{
    public class InventoryViewModel<TModel>
        :
        UI.InventoryViewModel<TModel>,
        IInventoryViewModel

        where TModel : IInventory
    {
        private readonly ObservableDictionary<IItemContainer, VisualElement> containerElements = new(4, new ReferenceEqualityComparer<IItemContainer>());

        private IDisposable? containerViewAddBinding;
        private IDisposable? containerViewRemoveBinding;
        private IDisposable? containerViewReplaceBinding;
        private IDisposable? containerViewClearBinding;

        public IReadOnlyObservableDictionary<IItemContainer, VisualElement> ContainerElements => containerElements;

        public InventoryViewModel(
            TModel? model,
            GameObject containerPrefab,
            Transform containersRoot
            )
            :
            base(
                model,
                containerPrefab,
                containersRoot
                )
        {
        }

        protected override void OnSetModel(TModel? model)
        {
            base.OnSetModel(model);
            CCDisposable.Dispose(ref containerViewAddBinding);
            CCDisposable.Dispose(ref containerViewRemoveBinding);
            CCDisposable.Dispose(ref containerViewReplaceBinding);
            CCDisposable.Dispose(ref containerViewClearBinding);
        }

        protected override void InitModel(TModel model)
        {
            base.InitModel(model);
            BindContainerViewAdd();
            BindContainerViewRemove();
            BindContainerViewsReplace();
            BindContainerViewsClear();
        }

        private async void OnContainerViewAdd(
            DictionaryAddEvent<IItemContainer, GameObject> addEv
            )
        {
            var (container, containerViewGO) = addEv;

            if (!containerViewGO.Q()
                    .Component<IElement>()
                    .Lax()
                    .TryGetValue(out var containerElement)
                )
            {
                return;
            }

            containerElement.ObserveRootElement()
                .Where(root => root.Current is not null)
                .Take(1)
                .Subscribe((@this: this, container),
                static (root, args) =>
                {
                    var (@this, container) = args;
                    @this.containerElements.Add(container, root.Current!);
                });
        }

        private void BindContainerViewAdd()
        {
            containerViewAddBinding = ContainerViews.ObserveDictionaryAdd(DisposeCancellationToken)
                .Subscribe(OnContainerViewAdd);
        }

        private void OnContainerViewRemove(
            DictionaryRemoveEvent<IItemContainer, GameObject> removeEv
            )
        {
            containerElements.Remove(removeEv.Key);
        }

        private void BindContainerViewRemove()
        {
            containerViewRemoveBinding = ContainerViews.ObserveDictionaryRemove(DisposeCancellationToken)
                .Subscribe(OnContainerViewRemove);
        }

        private void OnContainerViewReplace(
            DictionaryReplaceEvent<IItemContainer, GameObject> replaceEv
            )
        {
            var (container, oldContainerViewGO, newContainerViewGO) = replaceEv;

            var addEv = new DictionaryAddEvent<IItemContainer, GameObject>(container, oldContainerViewGO);
            var removeEv = new DictionaryRemoveEvent<IItemContainer, GameObject>(container, newContainerViewGO);

            OnContainerViewRemove(removeEv);
            OnContainerViewAdd(addEv);
        }

        private void BindContainerViewsReplace()
        {
            containerViewReplaceBinding = ContainerViews.ObserveDictionaryReplace(DisposeCancellationToken)
                .Subscribe(OnContainerViewReplace);
        }

        private void OnContainersViewsClear(Unit _)
        {
            containerElements.Clear();
        }

        private void BindContainerViewsClear()
        {
            containerViewClearBinding = ContainerViews.ObserveClear(DisposeCancellationToken)
                .Subscribe(OnContainersViewsClear);
        }
    }
}
