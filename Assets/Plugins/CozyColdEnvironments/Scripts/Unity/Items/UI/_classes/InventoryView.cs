using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using Cysharp.Threading.Tasks;
using SuperLinq;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UniRx;
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
        public GameObject ContainerPrefab;

        [GetByChildren]
        [SerializeField]
        protected GameObjectList slots;

        [SerializeField]
        protected int itemContainerCount;

        protected Dictionary<int, GameObject> instantiatedGameObjects = new();

        private int addCntOperationCount;

        public GameObjectList Slots => slots;
        public ISelectableController<IItemContainer> SelectableObserver { get; private set; } = null!;
        public CanvasGroup canvasGroup { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            SelectableObserver = this.QueryTo()
                .Component<ISelectableController<IItemContainer>>()
                .Lax()
                .GetValue(() => gameObject.AddComponent<ItemContainerViewSelectableObserver>());

            canvasGroup = this.Q()
                .Component<CanvasGroup>()
                .Lax()
                .GetValue(() => gameObject.AddComponent<CanvasGroup>());
        }

        protected override void Start()
        {
            base.Start();
            SetupOnAddSlotGameObject();
        }

        protected override void Init()
        {
            base.Init();
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

        private static async UniTask OnAddContainer(DictionaryAddEvent<int, IItemContainer> cnt,
            InventoryView<TViewModel> inst)
        {
            inst.addCntOperationCount++;
            inst.canvasGroup.alpha = 0;
            var go = (await InstantiateAsync(inst.ContainerPrefab, parent: inst.transform))[0];
            try
            {
                var view = go.QueryTo()
                    .FromChildrens()
                    .IncludeInactive()
                    .Views()
                    .First(view => view.model.Raw is IItemContainer);

                view.SetViewModelUnsafe(new ItemContainerViewModel<IItemContainer>(cnt.Value));
                inst.instantiatedGameObjects.Add(cnt.Key, go);
                inst.slots.Add(go);
            }
            catch (System.Exception ex)
            {
                Destroy(go);
                inst.PrintException(ex);
            }
            finally
            {
                inst.addCntOperationCount--;

                if (inst.addCntOperationCount <= 0)
                {
                    inst.canvasGroup.alpha = 1;
                    inst.GetRootGUI().IfSome(x => x.Redraw());
                }
            }
        }

        private void BindAddContainer()
        {
            if (this.viewModel.TryGetValue(out var viewModel))
            {
                viewModel.ObserveAdd()
                     .SubscribeWithState(this,
                     static (cnt, @this) =>
                     {
                         OnAddContainer(cnt, @this)
                             .AttachExternalCancellation(@this.destroyCancellationToken)
                             .Forget();
                     })
                     .AddTo(this);
            }
        }

        private void BindRemoveContainer()
        {
            viewModel.IfSome(viewModel =>
            {
                viewModel.ObserveRemove()
                     .SubscribeWithState(this,
                     static (cnt, @this) =>
                     {
                         if (@this.instantiatedGameObjects.Remove(cnt.Key, out GameObject go))
                             @this.slots.Remove(go);
                     })
                     .AddTo(this);
            });
        }

        private void BindResetInventory()
        {
            viewModel.IfSome(viewModel =>
            {
                viewModel.ObserveReset()
                .SubscribeWithState(this,
                    static (_, @this) =>
                    {
                        @this.instantiatedGameObjects.Clear();
                        @this.slots.Clear();
                    })
                    .AddTo(this);
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
                OnAddContainer(ev, this).AttachExternalCancellation(destroyCancellationToken).Forget();
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
