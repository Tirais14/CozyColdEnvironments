#nullable enable
using CCEnvs.Disposables;
using CCEnvs.Threading.Tasks;
using CCEnvs.UnityX.UI.Elements;
using Cysharp.Threading.Tasks;
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
        private readonly ObservableDictionary<IItemContainer, VisualElement> containerRendererRoots = new(4, new ReferenceEqualityComparer<IItemContainer>());

        private IDisposable? containerViewAddBinding;
        private IDisposable? containerViewRemoveBinding;
        private IDisposable? containerViewReplaceBinding;
        private IDisposable? containerViewClearBinding;

        public IReadOnlyObservableDictionary<IItemContainer, VisualElement> ContainerRendererRoots => containerRendererRoots;

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

        private async ValueTask OnContainerViewAdd(
            DictionaryAddEvent<CCEnvs.UnityX.Items.IItemContainer, GameObject> addEv,
            CancellationToken cancellationToken
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            var (container, containerViewGO) = addEv;

            if (!containerViewGO.Q().Component<IShowableElement>().Lax().TryGetValue(out var containerView))
                return;

            if (containerView.rendererRoot is null)
            {
                await UniTask.WaitUntil(
                    containerView,
                    static containerView =>
                    {
                        return containerView.rendererRoot is not null;
                    },
                    cancellationToken: cancellationToken
                    );
            }

            containerRendererRoots.Add(container, containerView.rendererRoot!);
        }

        private void BindContainerViewAdd()
        {
            containerViewAddBinding = ContainerViews.ObserveDictionaryAdd(DisposeCancellationToken)
                .SubscribeAwait(OnContainerViewAdd);
        }

        private void OnContainerViewRemove(
            DictionaryRemoveEvent<CCEnvs.UnityX.Items.IItemContainer, GameObject> removeEv
            )
        {
            containerRendererRoots.Remove((IItemContainer)removeEv.Key);
        }

        private void BindContainerViewRemove()
        {
            containerViewRemoveBinding = ContainerViews.ObserveDictionaryRemove(DisposeCancellationToken)
                .Subscribe(OnContainerViewRemove);
        }

        private void OnContainerViewReplace(
            DictionaryReplaceEvent<CCEnvs.UnityX.Items.IItemContainer, GameObject> replaceEv
            )
        {
            var (container, oldContainerViewGO, newContainerViewGO) = replaceEv;

            var addEv = new DictionaryAddEvent<CCEnvs.UnityX.Items.IItemContainer, GameObject>(container, oldContainerViewGO);
            var removeEv = new DictionaryRemoveEvent<CCEnvs.UnityX.Items.IItemContainer, GameObject>(container, newContainerViewGO);

            OnContainerViewRemove(removeEv);
            OnContainerViewAdd(addEv, DisposeCancellationToken).Forget();
        }

        private void BindContainerViewsReplace()
        {
            containerViewReplaceBinding = ContainerViews.ObserveDictionaryReplace(DisposeCancellationToken)
                .Subscribe(OnContainerViewReplace);
        }

        private void OnContainersViewsClear(Unit _)
        {
            containerRendererRoots.Clear();
        }

        private void BindContainerViewsClear()
        {
            containerViewClearBinding = ContainerViews.ObserveClear(DisposeCancellationToken)
                .Subscribe(OnContainersViewsClear);
        }
    }
}
