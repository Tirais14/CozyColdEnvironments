using CCEnvs.Collections;
using CCEnvs.Disposables;
using CCEnvs.UnityX.EditorSerialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.UI.Menus
{
    public abstract class ContextMenuView<TViewModel> : View<TViewModel>
        where TViewModel : IContextMenuViewModel
    {
        [SerializeField]
        protected Transform itemsRoot = null!;

        [MaybeNull]
        [SerializeField]
        protected GameObject? defaultItemViewPrefab = null!;

        [SerializeField]
        protected SerializedDictionary<string, GameObject> itemViewPrefabs = null!;

        private DisposableLight<(TViewModel, Action<IContextMenuItem>)> itemInvokeBinding;

        public Transform ItemsRoot {
            get => itemsRoot;
            set => itemsRoot = value.IfNull(transform);
        }

        public GameObject? DefaultItemViewPrefab {
            get => defaultItemViewPrefab;
            set => defaultItemViewPrefab = value;
        }

        public Dictionary<string, GameObject> ItemViewPrefabs => itemViewPrefabs.Data;

        protected override void Start()
        {
            base.Start();
            ItemsRoot = ItemsRoot;
        }

        protected virtual void OnItemInvoke(IContextMenuItem item) { }

        protected override void OnSetViewModel(TViewModel? viewModel)
        {
            CCDisposable.Dispose(ref itemInvokeBinding);
        }

        protected override void InitViewModel(TViewModel viewModel)
        {
            viewModel.OnItemInvoke += OnItemInvokeInternal;

            itemInvokeBinding = CCDisposable.Light(
                (viewModel, callback: (Action<IContextMenuItem>)OnItemInvokeInternal),
                static (args) => args.viewModel.OnItemInvoke -= args.callback
                );

            viewModel.DefaultItemViewPrefab = DefaultItemViewPrefab;
            viewModel.ItemViewPrefabs.Clear();
            viewModel.ItemViewPrefabs.AddRange(ItemViewPrefabs);
        }

        private void OnItemInvokeInternal(IContextMenuItem item)
        {
            if (TryGetModel<IContextMenu>(out var contextMenu))
                contextMenu.Clear();

            Showable.Hide();
            OnItemInvoke(item);
        }
    }
}
