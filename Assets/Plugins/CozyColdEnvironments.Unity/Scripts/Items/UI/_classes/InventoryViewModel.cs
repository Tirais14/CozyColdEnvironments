using CCEnvs.Collections;
using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.Linq;
using CCEnvs.Pools;
using CCEnvs.TypeMatching;
using CCEnvs.UnityX.Async;
using CCEnvs.UnityX.Items;
using CCEnvs.UnityX.UI;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;

#if ZLINQ_PLUGIN
using ZLinq;
#else
using System.Linq;
#endif

#nullable enable
namespace CCEnvs.UnityX.Items.UI
{
    public class InventoryViewModel<TModel>
        :
        ViewModel<TModel>,
        IInventoryViewModel

        where TModel : IInventory
    {
        private readonly ObservableDictionary<IItemContainer, GameObject> containerViews = new(4, new ReferenceEqualityComparer<IItemContainer>());

        /// <summary>
        /// Containers added from view
        /// </summary>
        private readonly Lazy<HashSet<IItemContainer>> fromViewContainers = new(() => new HashSet<IItemContainer>());

        private IDisposable? addContainerBinding;
        private IDisposable? removeContainerBinding;
        private IDisposable? replaceContainerBinding;
        private IDisposable? clearContainersBinding;

        //public IReadOnlyObservableDictionary<int, IItemContainer> Containers => GuardedModel.Containers;

        //public IReadOnlyObservableDictionary<int, IItemContainer> Containers => GuardedModel.Containers;

        public IReadOnlyObservableDictionary<IItemContainer, GameObject> ContainerViews => containerViews;

        public GameObject ContainerPrefab { get; }

        public Transform ContainersRoot { get; }

        public InventoryViewModel(
            TModel? model,
            GameObject containerPrefab,
            Transform containersRoot
            )
        {
            CC.Guard.IsNotNull(containerPrefab, nameof(containerPrefab));

            ContainerPrefab = containerPrefab;
            ContainersRoot = containersRoot;

            SetModel(model);
        }

        public void AddContainer(IItemContainer cnt)
        {
            fromViewContainers.Value.Add(cnt);
            GuardedModel.AddContainer(cnt);
        }

        public void RemoveContainer(int id) => GuardedModel.RemoveContainer(id);

        protected override void OnSetModel(TModel? model)
        {
            CCDisposable.Dispose(ref addContainerBinding);
            CCDisposable.Dispose(ref removeContainerBinding);
            CCDisposable.Dispose(ref replaceContainerBinding);
            CCDisposable.Dispose(ref clearContainersBinding);
        }

        protected override void InitModel(TModel model)
        {
            InitExistingContainers(model);
            BindContainerAdd(model);
            BindContainerRemove(model);
            BindContainerReplace(model);
            BindContainersClear(model);
        }

        private void InitExistingContainers(TModel model)
        {
            var existsingContainers = model.Containers
#if ZLINQ_PLUGIN
                .AsValueEnumerable()
#endif
                .Select(cnt => new InventoryContainerAddEvent { ID = cnt.Key, Container = cnt.Value })
                .ToArray();

            OnContainersAdd(existsingContainers);
        }

        private void BindContainerAdd(TModel model)
        {
            addContainerBinding = model.ObserveContainerAdd()
                .ChunkFrame(1)
                .Subscribe(OnContainersAdd);
        }

        private void OnContainersAdd(InventoryContainerAddEvent[] addEvs)
        {
            if (addEvs.IsEmpty())
                return;

            OnAddContainersCore(addEvs, DisposeCancellationToken).ForgetByPrintException();
        }

        private async UniTask OnAddContainersCore(
            InventoryContainerAddEvent[] addEvs,
            CancellationToken cancellationToken
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var containers = ListPool<IItemContainer>.Shared.Get(addEvs.Length);

            foreach (var addEv in addEvs)
            {
                if (fromViewContainers.TryGetValue(out var fromViewCnts)
                    &&
                    fromViewCnts.Contains(addEv.Container))
                {
                    fromViewContainers.Value.Remove(addEv.Container);
                    continue;
                }

                containers.Value.Add(addEv.Container);
            }

            var containerViewModels = await InstantiateContainers(containers.Value.Count, cancellationToken);

            foreach (var (container, containerViewModel) in containers.Value.EquiZip(containerViewModels))
                containerViewModel.SetModel(container);
        }

        private async UniTask<IReadOnlyList<IItemContainerViewModel>> InstantiateContainers(
            int count,
            CancellationToken cancellationToken
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (count <= 0)
                return Array.Empty<IItemContainerViewModel>();

            var instParams = new InstantiateParameters()
            {
                parent = ContainersRoot
            };

            var instances = await Object.InstantiateAsync(
                ContainerPrefab,
                count,
                instParams,
                cancellationToken: cancellationToken
                );

            var containerViewModels = new List<IItemContainerViewModel>(count);

            try
            {
                foreach (var go in instances)
                {
                    if (!go.Q()
                        .IncludeInactive()
                        .FromChildrens()
                        .Component<IView>()
                        .Lax()
                        .TryGetValue(out var view))
                    {
                        this.PrintError(DebugMessageBuilder.CreatePooled()
                            .AddMessage("Container game object view doesn't contains view component")
                            .AddProperty("GameObject", go)
                            .AddProperty("Archetype", go.GetComponents<Component>().Select(cmp => cmp.GetType().FullName).SequenceToString())
                            .ToStringAndDispose()
                            );
                        Object.Destroy(go);
                        continue;
                    }

                    if (!view.HasViewModel<IItemContainerViewModel>())
                    {
                        this.PrintError(DebugMessageBuilder.CreatePooled()
                            .AddMessage("View doesn't contains view model")
                            .AddProperty("GameObject", go)
                            .ToStringAndDispose()
                            );
                        Object.Destroy(go);
                        continue;
                    }

                    containerViewModels.Add(view.GetViewModel<IItemContainerViewModel>());

                    if (!view.HasModel<IItemContainer>())
                    {
                        this.PrintError(DebugMessageBuilder.CreatePooled()
                            .AddMessage("View doesn't contains model")
                            .AddProperty("GameObject", go)
                            .ToStringAndDispose()
                            );
                        Object.Destroy(go);
                        continue;
                    }

                    containerViews.Add(view.GetModel<IItemContainer>(), go);
                }

                return containerViewModels;
            }
            catch (Exception ex)
            {
                this.PrintException(ex);

                foreach (var go in instances)
                    UnityEngine.Object.Destroy(go);

                foreach (var containerViewModel in containerViewModels
#if ZLINQ_PLUGIN
                    .AsValueEnumerable()
#endif
                    .Select(cntVM => cntVM.Model)
                    .OfType<IItemContainer>()
                    )
                {
                    containerViews.Remove(containerViewModel);
                }
            }

            return Array.Empty<IItemContainerViewModel>();
        }

        private void BindContainerRemove(TModel model)
        {
            removeContainerBinding = model.ObserveContainerRemove()
                .ChunkFrame(1)
                .Subscribe(OnContainersRemove);
        }

        private void OnContainersRemove(InventoryContainerRemoveEvent[] removeEvs)
        {
            if (removeEvs.IsEmpty())
                return;

            IItemContainer cnt;

            foreach (var addEv in removeEvs)
            {
                cnt = addEv.Container;

                if (!containerViews.Remove(cnt, out var go))
                    continue;

                UnityEngine.Object.Destroy(go);
            }
        }

        private void BindContainerReplace(TModel model)
        {
            replaceContainerBinding = model.ObserveContainerReplace()
                .ChunkFrame(1)
                .Subscribe(OnContainerReplace);
        }

        private void OnContainerReplace(InventoryContainerReplaceEvent[] replaceEvs)
        {
            if (replaceEvs.IsEmpty())
                return;

            var removeEvs = new InventoryContainerRemoveEvent[replaceEvs.Length];
            var addEvs = new InventoryContainerAddEvent[replaceEvs.Length];

            InventoryContainerReplaceEvent replaceEv;

            for (int i = 0; i < replaceEvs.Length; i++)
            {
                replaceEv = replaceEvs[i];

                removeEvs[i] = new InventoryContainerRemoveEvent { ID = replaceEv.ID, Container = replaceEv.OldContainer };
                addEvs[i] = new InventoryContainerAddEvent { ID = replaceEv.ID, Container = replaceEv.NewContainer };
            }

            OnContainersRemove(removeEvs);
            OnContainersAdd(addEvs);
        }

        private void BindContainersClear(TModel model)
        {
            clearContainersBinding = model.ObserveClear()
                .Subscribe(OnContainersClear);
        }

        private void OnContainersClear(Unit _)
        {
            foreach (var cntView in containerViews.SelectValue())
                UnityEngine.Object.Destroy(cntView);
        }
    }
}
