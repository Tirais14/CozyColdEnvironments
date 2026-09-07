using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using R3;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

#nullable enable
namespace CCEnvs.UnityX.UI.Menus
{
    public class ContextMenuViewModel<TModel>
        : 
        ViewModel<TModel>,
        IContextMenuViewModel

        where TModel : IContextMenu
    {
        private readonly Dictionary<IContextMenuItem, IView> itemViews = new(ReferenceEqualityComparer<IContextMenuItem>.Default);

        private readonly Dictionary<IContextMenuItem, Action> itemCallbacks = new(ReferenceEqualityComparer<IContextMenuItem>.Default);

        private GameObject? defaultItemViewPrefab;

        private IDisposable? addBinding;
        private IDisposable? removeBinding;
        private IDisposable? replaceBinding;
        private IDisposable? clearBinding;

        public event Action<IContextMenuItem>? OnItemInvoke;

        public GameObject? DefaultItemViewPrefab {
            get => defaultItemViewPrefab;
            set => defaultItemViewPrefab = value;
        }

        public IDictionary<string, GameObject> ItemViewPrefabs { get; } = new Dictionary<string, GameObject>();

        public Transform ItemsRoot { get; }

        public ContextMenuViewModel(Transform itemsRoot)
        {
            CC.Guard.IsNotNull(itemsRoot, nameof(itemsRoot));

            ItemsRoot = itemsRoot;
        }

        protected override void OnSetModel(TModel? model)
        {
            CCDisposable.Dispose(ref addBinding);
            CCDisposable.Dispose(ref removeBinding);
            CCDisposable.Dispose(ref replaceBinding);
            CCDisposable.Dispose(ref clearBinding);
        }

        protected override void InitModel(TModel model)
        {
            BindItemAdd(model);
            BindItemRemove(model);
            BindItemReplace(model);
            BindItemsClear(model);
        }

        protected virtual void OnItemAdd(IContextMenuItem item) { }

        protected virtual void OnItemRemove(IContextMenuItem item) { }

        protected virtual void OnItemReplace(PreviousCurrentPair<IContextMenuItem> item) { }

        protected virtual void OnItemsClear() { }

        private void OnItemAddCore(IContextMenuItem item)
        {
            if (itemViews.TryGetValue(item, out IView? itemView))
            {
                itemView.Showable.Show();
                return;
            }

            if (!ItemViewPrefabs.TryGetValue(item.Name, out GameObject? itemViewPrefab))
            {
                if (DefaultItemViewPrefab == null)
                {
                    this.PrintError(DebugMessageBuilder.CreatePooled()
                    .AddMessage("Cannot find item prefab")
                    .AddProperty(nameof(item), item)
                    .ToStringAndDispose()
                    );
                    return;
                }

                itemViewPrefab = DefaultItemViewPrefab;
            }

            GameObject itemViewGameObject = Object.Instantiate(itemViewPrefab, ItemsRoot);

            if (!itemViewGameObject.Q().Component<IView>().Lax().TryGetValue(out itemView))
            {
                this.PrintError(DebugMessageBuilder.CreatePooled()
                    .AddMessage("Cannot find item view")
                    .AddProperty(nameof(itemViewGameObject), itemViewGameObject)
                    .ToStringAndDispose()
                    );

                Object.Destroy(itemViewGameObject);
                return;
            }

            itemView.Showable.Show();

            if (!itemView.TryGetViewModel<IContextMenuItemViewModel>(out var itemViewModel))
            {
                this.PrintError(DebugMessageBuilder.CreatePooled()
                    .AddMessage("Cannot find required view model")
                    .AddProperty(nameof(itemView), itemView)
                    .ToStringAndDispose()
                    );

                Object.Destroy(itemViewGameObject);
                return;
            }

            itemViews.Add(item, itemView);
            itemViewModel.SetModel(item);

            void itemCallback() => OnItemInvoke?.Invoke(item);
            itemCallbacks.Add(item, itemCallback);
            item.OnInvoke += itemCallback;
        }

        private void OnItemAddInternal(IContextMenuItem item)
        {
            OnItemAddCore(item);
            OnItemAdd(item);
        }

        private void BindItemAdd(TModel model)
        {
            addBinding = model.ObserveAdd()
                .Subscribe(OnItemAddInternal);
        }

        private void OnItemViewRemove(IView itemView)
        {
            itemView.IfNotNull(x => x.ViewModel).IfNotNull(viewModel => viewModel.SetModel(null));
            itemView.Showable.Hide();
        }

        private void OnItemRemoveCore(IContextMenuItem item)
        {
            if (itemViews.Remove(item, out IView? itemView))
                OnItemViewRemove(itemView);

            if (itemCallbacks.Remove(item, out Action? itemCallback))
                item.OnInvoke -= itemCallback;
        }

        private void OnItemRemoveInternal(IContextMenuItem item)
        {
            OnItemRemoveCore(item);
            OnItemRemove(item);
        }

        private void BindItemRemove(TModel model)
        {
            removeBinding = model.ObserveRemove().Subscribe(OnItemRemoveInternal);
        }

        private void OnItemReplaceInternal(PreviousCurrentPair<IContextMenuItem> item)
        {
            OnItemAddCore(item.Previous);
            OnItemRemoveCore(item.Current);
            OnItemReplace(item);
        }

        private void BindItemReplace(TModel model)
        {
            replaceBinding = model.ObserveReplace().Subscribe(OnItemReplaceInternal);
        }

        private void OnItemsClearInternal(Unit _)
        {
            foreach (var itemView in itemViews.Values)
                if (itemView.TryGetModel<IContextMenuItem>(out var item))
                    OnItemRemove(item);

            foreach (var itemView in itemViews.Values)
                OnItemViewRemove(itemView);

            itemViews.Clear();
            itemCallbacks.Clear();

            OnItemsClear();
        }

        private void BindItemsClear(TModel model)
        {
            clearBinding = model.ObserveClear().Subscribe(OnItemsClearInternal);
        }
    }
}
