using CCEnvs.Disposables;
using CommunityToolkit.Diagnostics;
using ObservableCollections;
using R3;
using System;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.Items.UIElements
{
    public abstract class InventoryView<TViewModel>
        :
        UI.InventoryView<TViewModel>

        where TViewModel : IInventoryViewModel
    {
        [SerializeField]
        //[Tooltip("Element type must be ScrollView")]
        protected string? containerElementName = "containers";

        protected VisualElement? containersView;

        private IDisposable? viewModelContainerRootAddBinding;
        private IDisposable? viewModelContainerRootRemoveBinding;
        private IDisposable? viewModelContainerRootReplaceBinding;
        private IDisposable? viewModelContainerRootsClearBinding;

        protected override void OnSetViewModel(TViewModel? vm)
        {
            if (vm.IsNull())
                ElementShowable.Renderer.UnregisterUIReloadCallback(OnUIReload);
            else
                ElementShowable.Renderer.RegisterUIReloadCallback(OnUIReload);

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
            Guard.IsNotNull(containersView, nameof(containersView));
            containersView.Add(addEv.Value);
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
            containersView?.Remove(removeEv.Value);
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

        private void BindViewModelContainerRootReplace()
        {
            viewModelContainerRootReplaceBinding = GuardedViewModel.ContainerRendererRoots.ObserveDictionaryReplace(destroyCancellationToken)
                .Subscribe(OnViewModelContainerRootReplace);
        }

        private void OnViewModelContainerRootsClear(Unit _)
        {
            containersView?.Clear();
        }

        private void BindViewModelContainerRootsClear()
        {
            viewModelContainerRootsClearBinding = GuardedViewModel.ContainerRendererRoots.ObserveClear(destroyCancellationToken)
                .Subscribe(OnViewModelContainerRootsClear);
        }

        private void OnUIReload(PanelRenderer _, VisualElement root)
        {
            containersView = root.Q<VisualElement>(containerElementName);
            containersView.Clear();
        }
    }
}
