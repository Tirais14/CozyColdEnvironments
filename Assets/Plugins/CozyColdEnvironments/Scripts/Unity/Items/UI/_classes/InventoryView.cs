using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
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

            if (this.model.Cast<IInventory>().TryGetRightValue(out var model))
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
                    .First(view => view.model.Raw is IItemContainer);

                view.SetViewModelUnsafe(new ItemContainerViewModel<IItemContainer>(cnt.Value));
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
                    root.IfSome(x => x.Redraw());
            }
        }

        private void BindAddContainer()
        {
            if (this.viewModel.TryGetValue(out var viewModel))
            {
                viewModel.ObserveAddContainer()
                     .Subscribe(this,
                     static (cnt, @this) =>
                     {
                         @this.OnAddContainer(cnt)
                             .AttachExternalCancellation(@this.destroyCancellationToken)
                             .Forget();
                     })
                     .AddToBehaviour(this);
            }
        }

        private void BindRemoveContainer()
        {
            viewModel.IfSome(viewModel =>
            {
                viewModel.ObserveRemoveContainer()
                     .Subscribe(this,
                     static (cnt, @this) =>
                     {
                         if (@this.instantiatedGameObjects.Remove(cnt.Key, out GameObject go))
                             @this.slots.Remove(go);
                     })
                     .AddToBehaviour(this);
            });
        }

        private void BindResetInventory()
        {
            viewModel.IfSome(viewModel =>
            {
                viewModel.ObserveResetContainer()
                .Subscribe(this,
                    static (_, @this) =>
                    {
                        @this.instantiatedGameObjects.Clear();
                        @this.slots.Clear();
                    })
                    .AddToBehaviour(this);
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
            if (!modelUnsafe.To<IInventory>().Any())
                return;

            foreach (var ev in modelUnsafe.To<IInventory>()
                .ZLinq()
                .Select(pair => new DictionaryAddEvent<int, IItemContainer>(pair.Key, pair.Value)))
            {
                OnAddContainer(ev).AttachExternalCancellation(destroyCancellationToken).Forget();
            }
        }
    }
    public class InventoryView : InventoryView<InventoryViewModel<IInventory>>
    {
        protected override Maybe<InventoryViewModel<IInventory>> ViewModelFactory()
        {
            var inv = new Inventory(itemContainerCount);
            return new InventoryViewModel<IInventory>(inv);
        }
    }
}
