using CCEnvs.Collections;
using CCEnvs.Disposables;
using CCEnvs.Linq;
using CCEnvs.Pools;
using CCEnvs.TypeMatching;
using CCEnvs.Unity.Async;
using CCEnvs.Unity.Items;
using CCEnvs.Unity.UI;
using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using SuperLinq;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

#if ZLINQ_PLUGIN
using ZLinq;
#else
using System.Linq;
#endif

#nullable enable
namespace CCEnvs.Unity.Storages.UI
{
    public class InventoryViewModel<TModel>
        :
        ViewModel<TModel>, 
        IInventoryViewModel

        where TModel : IInventory
    {
        private readonly ObservableDictionary<IItemContainer, GameObject> containerViews = new();
        private readonly Lazy<HashSet<IItemContainer>> fromViewContainers = new(() => new HashSet<IItemContainer>());

        private IDisposable? addContainerBinding;
        private IDisposable? removeContainerBinding;
        private IDisposable? replaceContainerBinding;
        private IDisposable? clearContainersBinding;

        public IReadOnlyObservableDictionary<int, IItemContainer> Containers => GuardedModel.Containers;

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
            InitExistingContainers();
            BindContainerAdd();
            BindContainerRemove();
            BindContainerReplace();
            BindContainersClear();
        }

        private void InitExistingContainers()
        {
            var existsingCnts = Containers
#if ZLINQ_PLUGIN
                .AsValueEnumerable()
#endif
                .Select(cnt => new DictionaryAddEvent<int, IItemContainer>(cnt.Key, cnt.Value))
                .ToArray();

            OnContainersAdd(existsingCnts);
        }

        private void BindContainerAdd()
        {
            addContainerBinding = Containers.ObserveDictionaryAdd(DisposeCancellationToken)
                .ChunkFrame(1)
                .Subscribe(OnContainersAdd);
        }

        private void OnContainersAdd(DictionaryAddEvent<int, IItemContainer>[] addEvs)
        {
            if (addEvs.IsEmpty())
                return;

            OnAddContainersCore(addEvs, DisposeCancellationToken).ForgetByPrintException();
        }

        private async UniTask OnAddContainersCore(
            DictionaryAddEvent<int, IItemContainer>[] addEvs, 
            CancellationToken cancellationToken
            )
        {
            cancellationToken.ThrowIfCancellationRequested();

            using var cnts = ListPool<IItemContainer>.Shared.Get();
            cnts.Value.TryIncreaseCapacity(addEvs.Length);

            foreach (var addEv in addEvs)
            {
                if (fromViewContainers.TryGetValue(out var fromViewCnts)
                    &&
                    fromViewCnts.Contains(addEv.Value))
                {
                    fromViewContainers.Value.Remove(addEv.Value);
                    continue;
                }

                cnts.Value.Add(addEv.Value);
            }

            var cntViewModels = await InstantiateContainers(cnts.Value.Count, cancellationToken);

            foreach (var (cnt, cntVM) in cnts.Value.EquiZip(cntViewModels))
                cntVM.SetModel(cnt);
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

            var instances = await UnityEngine.Object.InstantiateAsync(
                ContainerPrefab,
                count,
                instParams,
                cancellationToken: cancellationToken
                );

            var cntViewModels = new List<IItemContainerViewModel>();

            try
            {
                foreach (var go in instances)
                {
                    if (!go.Q()
                        .IncludeInactive()
                        .FromChildrens()
                        .Component<IView>()
                        .Lax()
                        .TryGetValue(out var view)
                        ||
                        view.ViewModel.IsNot<IItemContainerViewModel>(out var cntViewModel)
                        ||
                        view.Model.IsNot<IItemContainer>(out var cnt))
                    {
                        continue;
                    }

                    cntViewModels.Add(cntViewModel);
                    containerViews.Add(cnt, go);
                }

                return cntViewModels;
            }
            catch (Exception ex)
            {
                this.PrintException(ex);

                foreach (var go in instances)
                    UnityEngine.Object.Destroy(go);

                foreach (var cnt in cntViewModels
#if ZLINQ_PLUGIN
                    .AsValueEnumerable()
#endif
                    .Select(cntVM => cntVM.Model)
                    .OfType<IItemContainer>()
                    )
                {
                    containerViews.Remove(cnt);
                }
            }

            return Array.Empty<IItemContainerViewModel>();
        }

        private void BindContainerRemove()
        {
            removeContainerBinding = Containers.ObserveDictionaryRemove(DisposeCancellationToken)
                .ChunkFrame(1)
                .Subscribe(OnContainersRemove);
        }

        private void OnContainersRemove(DictionaryRemoveEvent<int, IItemContainer>[] removeEvs)
        {
            if (removeEvs.IsEmpty())
                return;

            IItemContainer cnt;

            foreach (var addEv in removeEvs)
            {
                cnt = addEv.Value;

                if (!containerViews.Remove(cnt, out var go))
                    continue;

                UnityEngine.Object.Destroy(go);
            }
        }

        private void BindContainerReplace()
        {
            replaceContainerBinding = Containers.ObserveDictionaryReplace(DisposeCancellationToken)
                .ChunkFrame(1)
                .Subscribe(OnContainerReplace);
        }

        private void OnContainerReplace(DictionaryReplaceEvent<int, IItemContainer>[] replaceEvs)
        {
            if (replaceEvs.IsEmpty())
                return;

            var removeEvs = new DictionaryRemoveEvent<int, IItemContainer>[replaceEvs.Length];
            var addEvs = new DictionaryAddEvent<int, IItemContainer>[replaceEvs.Length];

            DictionaryReplaceEvent<int, IItemContainer> replaceEv;

            for (int i = 0; i < replaceEvs.Length; i++)
            {
                replaceEv = replaceEvs[i];

                removeEvs[i] = new DictionaryRemoveEvent<int, IItemContainer>(replaceEv.Key, replaceEv.OldValue);
                addEvs[i] = new DictionaryAddEvent<int, IItemContainer>(replaceEv.Key, replaceEv.NewValue);
            }

            OnContainersRemove(removeEvs);
            OnContainersAdd(addEvs);
        }

        private void BindContainersClear()
        {
            clearContainersBinding = Containers.ObserveClear(DisposeCancellationToken)
                .Subscribe(OnContainersClear);
        }

        private void OnContainersClear(Unit _)
        {
            foreach (var cntView in containerViews.SelectValue())
                UnityEngine.Object.Destroy(cntView);
        }
    }
}
