using CCEnvs.Disposables;
using CCEnvs.UnityX.Injections;
using CommunityToolkit.Diagnostics;
using ObservableCollections;
using R3;
using System;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.Items.UIElements
{
    [RequireComponent(typeof(PanelRenderer))]
    public abstract class InventoryView<TViewModel>
        :
        UI.InventoryView<TViewModel>,
        IViewElement

        where TViewModel : IInventoryViewModel
    {
        [GetBySelf]
        protected PanelRenderer renderer;

        private IDisposable? viewModelContainerRootAddBinding;
        private IDisposable? viewModelContainerRootRemoveBinding;
        private IDisposable? viewModelContainerRootReplaceBinding;
        private IDisposable? viewModelContainerRootsClearBinding;

        public PanelRenderer Renderer => renderer;

        public VisualElement? RendererRoot { get; private set; }

        protected override void OnSetViewModel(TViewModel? vm)
        {
            if (vm is null)
            {
                renderer.UnregisterUIReloadCallback(OnUIReload);
                RendererRoot = null;
            }
            else
                renderer.RegisterUIReloadCallback(OnUIReload);

            CCDisposable.Dispose(ref viewModelContainerRootAddBinding);
            CCDisposable.Dispose(ref viewModelContainerRootRemoveBinding);
            CCDisposable.Dispose(ref viewModelContainerRootReplaceBinding);
            CCDisposable.Dispose(ref viewModelContainerRootsClearBinding);
        }

        protected override void InitViewModel(TViewModel vm)
        {
            BindViewModelContainerRootAdd();
            BindViewModelContainerRootRemove();
            BindViewModelContainerRootReplace();
            BindViewModelContainerRootsClear();
        }

        private void OnViewModelContainerRootAdd(
            DictionaryAddEvent<IItemContainer, VisualElement> addEv
            )
        {
            Guard.IsNotNull(RendererRoot);
            RendererRoot.Add(addEv.Value);
        }

        private void BindViewModelContainerRootAdd()
        {
            viewModelContainerRootAddBinding = GuardedViewModel.ContainerRendererRoots.ObserveDictionaryAdd(destroyCancellationToken)
                .Subscribe(OnViewModelContainerRootAdd);
        }

        private void OnViewModelContainerRootRemove(
            DictionaryRemoveEvent<IItemContainer, VisualElement> removeEv
            )
        {
            RendererRoot?.Remove(removeEv.Value);
        }

        private void BindViewModelContainerRootRemove()
        {
            viewModelContainerRootRemoveBinding = GuardedViewModel.ContainerRendererRoots.ObserveDictionaryRemove(destroyCancellationToken)
                .Subscribe(OnViewModelContainerRootRemove);
        }

        private void OnViewModelContainerRootReplace(DictionaryReplaceEvent<IItemContainer, VisualElement> replaceEv)
        {
            var removeEv = new DictionaryRemoveEvent<IItemContainer, VisualElement>(replaceEv.Key, replaceEv.OldValue);
            OnViewModelContainerRootRemove(removeEv);

            var addEv = new DictionaryAddEvent<IItemContainer, VisualElement>(replaceEv.Key, replaceEv.NewValue);
            OnViewModelContainerRootAdd(addEv);
        }

        private void OnViewModelContainerRootReplace()
        {
            viewModelContainerRootReplaceBinding = GuardedViewModel.ContainerRendererRoots.ObserveDictionaryReplace(destroyCancellationToken)
                .Subscribe(OnViewModelContainerRootReplace);
        }

        private void BindViewModelContainerRootReplace()
        {
            viewModelContainerRootReplaceBinding = GuardedViewModel.ContainerRendererRoots.ObserveDictionaryReplace(destroyCancellationToken)
                .Subscribe(OnViewModelContainerRootReplace);
        }

        private void OnViewModelContainerRootsClear(Unit _)
        {
            RendererRoot?.Clear();
        }

        private void BindViewModelContainerRootsClear()
        {
            viewModelContainerRootsClearBinding = GuardedViewModel.ContainerRendererRoots.ObserveClear(destroyCancellationToken)
                .Subscribe(OnViewModelContainerRootsClear);
        }

        private void OnUIReload(PanelRenderer renderer, VisualElement root)
        {
            RendererRoot = root;
            var containersView = root.Q<ScrollView>("containers");

            containersView.Clear();
        }
    }
}
