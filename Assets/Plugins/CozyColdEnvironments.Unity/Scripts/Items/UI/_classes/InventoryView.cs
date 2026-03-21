using System;
using System.Collections.Generic;
using System.Linq;
using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using UnityEditor;
using UnityEngine;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.Storages.UI
{
    public abstract class InventoryView<TViewModel>
        : View<TViewModel>

        where TViewModel : IInventoryViewModel
    {
        [Header("Inventory View")]
        [Space(8)]

        public GameObject ContainerPrefab;

        [GetByChildren]
        [SerializeField]
        protected GameObjectList slots;

        [SerializeField]
        protected int itemContainerCount;

        [SerializeField]
        protected bool inventoryAutoSize;

        protected Dictionary<int, GameObject> instantiatedGameObjects = new();

        [NonSerialized]
        private int addCntOperationCount;

        public GameObjectList Slots => slots;
        public ISelectableController<IItemContainer> SelectableObserver { get; private set; } = null!;

        protected override void Awake()
        {
            base.Awake();

            SelectableObserver = this.QueryTo()
                .Component<ISelectableController<IItemContainer>>()
                .Lax()
                .GetValue(() => gameObject.AddComponent<ItemContainerViewSelectableObserver>());
        }

        protected override void Start()
        {
            base.Start();
            SetupOnAddSlotGameObject();
        }

        protected override void Init()
        {
            base.Init();

            if (this.model.As<IInventory>().IsNotNull(out var model))
                model.AutoSize = inventoryAutoSize;

            InitItemContainers();
            BindAddContainer();
            BindRemoveContainer();
            BindResetInventory();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Slots.Clear();
        }

        protected virtual async UniTask OnAddContainer(DictionaryAddEvent<int, IItemContainer> cnt)
        {
            addCntOperationCount++;
            var go = (await InstantiateAsync(ContainerPrefab, parent: transform))[0];
            try
            {
                var view = go.QueryTo()
                    .FromChildrens()
                    .IncludeInactive()
                    .Views()
                    .First(view => view.model is IItemContainer);

                view.SetViewModel(new ItemContainerViewModel<IItemContainer>(cnt.Value, destroyCancellationToken));
                instantiatedGameObjects.Add(cnt.Key, go);
                slots.Add(go);
            }
            catch (Exception ex)
            {
                Destroy(go);
                this.PrintException(ex);
            }
            finally
            {
                addCntOperationCount--;

                if (addCntOperationCount <= 0)
                    root.Maybe().IfSome(x => x.Redraw());
            }
        }

        private void BindAddContainer()
        {
            if (this.viewModel.IsNotNull(out var viewModel))
            {
                viewModel.ObserveAdd()
                     .Subscribe(this,
                     static (cnt, @this) =>
                     {
                         @this.OnAddContainer(cnt)
                             .AttachExternalCancellation(@this.destroyCancellationToken)
                             .Forget();
                     })
                     .AddDisposableTo(this);
            }
        }

        private void BindRemoveContainer()
        {
            viewModel.IfNotNull(viewModel =>
            {
                viewModel.ObserveRemove()
                     .Subscribe(this,
                     static (cnt, @this) =>
                     {
                         if (@this.instantiatedGameObjects.Remove(cnt.Key, out GameObject go))
                             @this.slots.Remove(go);
                     })
                     .AddDisposableTo(this);
            });
        }

        private void BindResetInventory()
        {
            viewModel.IfNotNull(viewModel =>
            {
                viewModel.ObserveReset()
                .Subscribe(this,
                    static (_, @this) =>
                    {
                        @this.instantiatedGameObjects.Clear();
                        @this.slots.Clear();
                    })
                    .AddDisposableTo(this);
            });
        }

        private void SetupOnAddSlotGameObject()
        {
            slots.settings |= IGameObjectBag.Settings.DestroyOnRemove;
            //Slots.ObserveAdd()
            //     .SubscribeWithState(this, static (_, @this) => @this.Redraw())
            //     .AddTo(this);
        }

        private void InitItemContainers()
        {
            if (!modelUnsafe.CastTo<IInventory>().Any())
                return;

            foreach (var ev in modelUnsafe.CastTo<IInventory>()
                .AsValueEnumerable()
                .Select(pair => new DictionaryAddEvent<int, IItemContainer>(pair.Key, pair.Value)))
            {
                OnAddContainer(ev).AttachExternalCancellation(destroyCancellationToken).Forget();
            }
        }
    }
    public class InventoryView : InventoryView<InventoryViewModel<IInventory>>
    {
        protected override Maybe<InventoryViewModel<IInventory>> CreateViewModel()
        {
            var inv = new Inventory(itemContainerCount);
            return new InventoryViewModel<IInventory>(inv, destroyCancellationToken);
        }
    }
}
