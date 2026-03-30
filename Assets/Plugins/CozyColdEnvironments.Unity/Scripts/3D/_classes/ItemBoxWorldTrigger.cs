//using CCEnvs.Disposables;
//using CCEnvs.Threading;
//using CCEnvs.Unity.Components;
//using CCEnvs.Unity.Injections;
//using Cysharp.Threading.Tasks;
//using R3;
//using System;
//using System.Collections.Generic;
//using UnityEngine;

//#nullable enable
//namespace CCEnvs.Unity.D3
//{
//    [RequireComponent(typeof(ItemBox))]
//    public sealed class ItemBoxWorldTrigger : CCBehaviour
//    {
//        private readonly Dictionary<Collider, ValueReference<int>> itemColliders = new();

//        private readonly object recentlyRemovedItemsGate = new();

//        private IDisposable? boxItemRemoveBinding;

//        [field: GetBySelf]
//        public ItemBox ItemBox { get; private set; } = null!;

//        public string? TagFilter { get; set; }

//        protected override void Start()
//        {
//            base.Start();
//            BindBoxOnRemoveItem();
//            OnLateUpdate().Forget();
//        }

//        private async UniTaskVoid OnLateUpdate()
//        {
//            try
//            {
//                await UniTask.SwitchToThreadPool();

//                long frameCount = 0;

//                while (true)
//                {
//                    destroyCancellationToken.ThrowIfCancellationRequestedByIntervalAndMoveNext(ref frameCount, 30);

//                    await UniTask.WaitForSeconds(
//                        300f,
//                        ignoreTimeScale: true,
//                        PlayerLoopTiming.LastUpdate,
//                        destroyCancellationToken
//                        );

//                    lock (recentlyRemovedItemsGate)
//                        foreach (var item in recentlyRemovedItems.Keys)
//                            if (item == null)
//                                recentlyRemovedItems.Remove(item!);

//                }
//            }
//            catch (Exception ex)
//            {
//                if (ex.IsOperationCanceledException())
//                    return;

//                this.PrintException(ex);
//            }
//        }

//        private void OnTriggerEnter(Collider other)
//        {
//            if ((TagFilter is not null && !other.CompareTag(TagFilter))
//                ||
//                other.GetComponent<IBoxItem>().IsNull(out var item))
//            {
//                return;
//            }

//            lock (recentlyRemovedItemsGate)
//            {
//                if (itemColliders)
//            }

//            ItemBox.TryPushItem(item);
//        }

//        private void OnTriggerExit(Collider other)
//        {
//            lock (recentlyRemovedItemsGate)
//                recentlyRemovedItems.Remove(other);
//        }

//        protected override void OnDestroy()
//        {
//            base.OnDestroy();
//            CCDisposable.Dispose(ref boxItemRemoveBinding);
//        }

//        private void BindBoxOnRemoveItem()
//        {
//            boxItemRemoveBinding = ItemBox.ObserveSpawnItem()
//                .SubscribeOnThreadPool()
//                .Merge(ItemBox.ObservePopItem())
//                .Subscribe(OnBoxItemRemoved);
//        }

//        private void OnBoxItemRemoved(IBoxItem item)
//        {
//            lock (recentlyRemovedItemsGate)
//            {
//                if (recentlyRemovedItems.TryGetValue(item.collider, out var counter)
//                    &&
//                    counter <= 0)
//                {
//                    recentlyRemovedItems.Remove(item.collider);
//                }
//            }
//        }
//    }
//}
