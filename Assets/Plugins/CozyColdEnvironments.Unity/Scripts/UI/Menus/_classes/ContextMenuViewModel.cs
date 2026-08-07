using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.UnityX.Pools;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

        private IDisposable? addBinding;
        private IDisposable? removeBinding;
        private IDisposable? replaceBinding;
        private IDisposable? clearBinding;

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

        private async ValueTask OnItemAddInternal(
            IContextMenuItem item,
            CancellationToken cancellationToken
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!ItemViewPrefabs.TryGetValue(item.Name, out GameObject? itemViewPrefab))
            {
                this.PrintError(DebugMessageBuilder.CreatePooled()
                    .AddMessage("Cannot find item prefab")
                    .AddProperty(nameof(item), item)
                    .ToStringAndDispose()
                    );
                return;
            }

            GameObject[] itemViewGameObjects = await UnityObjectHelper.InstantiateAsync(
                itemViewPrefab,
                parameters: new InstantiateParameters
                {
                    parent = ItemsRoot
                },
                cancellationToken: DisposeCancellationToken
                );

            for (int i = 0; i < itemViewGameObjects.Length; i++)
            {
                GameObject itemViewGameObject = itemViewGameObjects[i];

                if (!itemViewGameObject.Q().Component<IView>().Lax().TryGetValue(out var itemView))
                {
                    this.PrintError(DebugMessageBuilder.CreatePooled()
                        .AddMessage("Cannot find item view")
                        .AddProperty(nameof(itemViewGameObject), itemViewGameObject)
                        .ToStringAndDispose()
                        );

                    continue;
                }

                if (!itemView.HasViewModel<IContextMenuItemViewModel>())
                {
                    this.PrintError(DebugMessageBuilder.CreatePooled()
                        .AddMessage("Cannot find required view model")
                        .AddProperty(nameof(itemView), itemView)
                        .ToStringAndDispose()
                        );

                    continue;
                }

                itemViews.Add(item, itemView);
                itemView.GetViewModel<IContextMenuItemViewModel>().SetModel(item);
            }

            OnItemAdd(item);
        }

        private void BindItemAdd(TModel model)
        {
            addBinding = model.ObserveAdd()
                .SubscribeAwait(OnItemAddInternal);
        }

        private void OnItemViewRemove(IView itemView)
        {
            itemView.As<Component>().IfNotNull(x => Object.Destroy(x));
        }

        private void OnItemRemoveInternal(IContextMenuItem item)
        {
            if (itemViews.Remove(item, out IView? itemView))
                OnItemViewRemove(itemView);

            OnItemRemove(item);
        }

        private void BindItemRemove(TModel model)
        {
            removeBinding = model.ObserveRemove().Subscribe(OnItemRemoveInternal);
        }

        private void OnItemReplaceInternal(PreviousCurrentPair<IContextMenuItem> item)
        {
            OnItemRemove(item.Previous);
            OnItemAdd(item.Current);
            OnItemReplace(item);
        }

        private void BindItemReplace(TModel model)
        {
            replaceBinding = model.ObserveReplace().Subscribe(OnItemReplaceInternal);
        }

        private void OnItemsClearInternal(Unit _)
        {
            foreach (var itemView in itemViews.Values)
                if (itemView.HasModel<IContextMenuItem>())
                    OnItemRemove(itemView.GetModel<IContextMenuItem>());

            foreach (var itemView in itemViews.Values)
                OnItemViewRemove(itemView);

            itemViews.Clear();

            OnItemsClear();
        }

        private void BindItemsClear(TModel model)
        {
            clearBinding = model.ObserveClear().Subscribe(OnItemsClearInternal);
        }
    }
}
