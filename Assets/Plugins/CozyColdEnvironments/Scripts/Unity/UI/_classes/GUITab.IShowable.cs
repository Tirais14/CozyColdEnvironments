using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Patterns.Commands;
using CCEnvs.Patterns.Factories;
using CCEnvs.Pools;
using CCEnvs.Snapshots;
using CCEnvs.Unity.Snapshots.UI;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public partial class GUITab : IShowable
    {
        private readonly static Lazy<ObjectPool<HideCommand>> hideCmdPool = new(
            static () =>
            {
                return new ObjectPool<HideCommand>(
                    factory: Factory.DefaultValueFactory<HideCommand>(),
                    capacity: 16
                    );
            });

        private readonly static Lazy<ObjectPool<ShowCommand>> showCmdPool = new(
            static () =>
            {
                return new ObjectPool<ShowCommand>(
                    factory: Factory.DefaultValueFactory<ShowCommand>(),
                    capacity: 16
                    );
            });

        private readonly static Lazy<ObjectPool<RedrawCommand>> redrawCmdPool = new(
            static () =>
            {
                return new ObjectPool<RedrawCommand>(
                    factory: Factory.DefaultValueFactory<RedrawCommand>(),
                    capacity: 16
                    );
            });

        [Header("Showable settings")]
        [Space(8)]

        [SerializeField]
        protected bool m_ShowOnInited;

        [SerializeField]
        protected ShowableRenderMode showableRenderMode;

        [NonSerialized]
        private readonly ReactiveProperty<bool> isShown = new(true);

        [NonSerialized]
        private readonly Lazy<Dictionary<object, ISnapshot>> snapshots = new(() => new Dictionary<object, ISnapshot>());

        [NonSerialized]
        private bool stateTransitioning;

        public bool ShowOnInited {
            get => m_ShowOnInited;
            set => m_ShowOnInited = value;
        }

        public bool IsShown => isShown.Value;
        public bool IsInited { get; private set; }

        private void IShowableAwake()
        {
            if (canvasGroup.IsNone
                &&
                showableRenderMode == ShowableRenderMode.CanvasGroup)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void IShowableStart()
        {
            InitShowableAsync().Forget();
        }

        private void IShowableOnDestroy()
        {
            isShown.Dispose();
        }

        public void Hide()
        {
            hideCmdPool.Value.Get().Value
                .SetGUITab(this)
                .ScheduleBy(commandScheduler);
        }

        public void Show()
        {
            showCmdPool.Value.Get().Value
                .SetGUITab(this)
                .ScheduleBy(commandScheduler);
        }

        public bool SwitchShownState()
        {
            if (IsShown)
                Hide();
            else
                Show();

            return IsShown;
        }

        public void SwitchShownStateVoid() => SwitchShownState();

        public void Redraw()
        {
            redrawCmdPool.Value.Get().Value
                .SetGUITab(this)
                .ScheduleBy(commandScheduler);
        }

        public Observable<Unit> ObserveShow()
        {
            return isShown.Where(static x => x).AsUnitObservable();
        }

        public Observable<Unit> ObserveHide()
        {
            return isShown.Where(static x => !x).AsUnitObservable();
        }

        protected virtual void OnHide()
        {
            stateTransitioning = true;
        }

        protected virtual void DoHide()
        {
            switch (showableRenderMode)
            {
                case ShowableRenderMode.GameObject:
                    {
                        gameObject.SetActive(false);
                        SetHiden();
                    }
                    break;

                case ShowableRenderMode.CanvasGroup:
                    {
                        var canvasGroup = this.canvasGroup.GetValueUnsafe(static () => throw new InvalidOperationException("Canvas group not found."));

                        snapshots.Value.Add(canvasGroup, new CanvasGroupSnapshot(canvasGroup));

                        canvasGroup.alpha = 0f;
                        canvasGroup.blocksRaycasts = false;
                        canvasGroup.interactable = false;

                        int iterationsPassed = 0;

                        foreach (var showable in this.Q()
                            .FromChildrens()
                            .FirstComponentsOnBranch()
                            .Components<IShowable>())
                        {
                            destroyCancellationToken.ThrowIfCancellationRequestedByIntervalAndMoveNext(ref iterationsPassed);

                            snapshots.Value.Add(showable, new ShowableSnapshot(showable));
                            showable.Hide();
                        }

                        SetHiden();
                    }
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }

        protected virtual void OnHiden()
        {
        }

        protected virtual void OnShow()
        {
            stateTransitioning = true;
        }

        protected virtual void DoShow()
        {
            switch (showableRenderMode)
            {
                case ShowableRenderMode.GameObject:
                    {
                        gameObject.SetActive(true);
                        SetShown();
                    }
                    break;
                case ShowableRenderMode.CanvasGroup:
                    {
                        using var snapshotsCopyHandle = snapshots.Value.ToArrayPooled();

                        foreach (var pair in snapshotsCopyHandle.Value)
                        {
                            pair.Value.TryRestore(pair.Key, out _);
                            snapshots.Value.Remove(pair.Key);
                        }

                        SetShown();
                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        protected virtual void OnShown()
        {
        }

        private void SetShown()
        {
            stateTransitioning = false;
            isShown.Value = true;

            OnShown();
        }

        private void SetHiden()
        {
            stateTransitioning = false;
            isShown.Value = false;

            OnHiden();
        }

        private async UniTask InitVisibleStateAsync(bool hasParent)
        {
            if (!ShowOnInited
                &&
                !hasParent)
            {
                Hide();
            }

            await UniTask.WaitWhile(this,
                predicate: static @this =>
                {
                    return @this.stateTransitioning;
                },
                cancellationToken: destroyCancellationToken
                );
        }

        private async UniTask WaitUntilChildrensInitedAsync()
        {
            var childs = this.Q()
                .FromChildrens()
                .ExcludeSelf()
                .Components<IShowable>();

            if (childs.IsNotEmpty())
            {
                await UniTask.WaitUntil(
                    childs,
                    static childs =>
                    {
                        return childs.All(x => x.IsInited);
                    },
                    cancellationToken: destroyCancellationToken
                    );
            }
        }

        private IShowable[] GetShowableControlledChilds()
        {
            return this.Q()
                .FromChildrens()
                .ExcludeSelf()
                .FirstComponentsOnBranch()
                .Components<IShowable>()
                .ToArray();
        }

        private RectTransform[] GetControlledLayouts()
        {
            return this.Q()
                .FromChildrens()
                .DepthLimiter<IShowable>()
                .Components<RectTransform>()
                .ToArray();
        }

        private async UniTask RebuildControlledLayouts()
        {
            var childs = GetControlledLayouts();

            var returnToNormalCanvas = CanvasHelper.MoveToDevCanvas(this.RectTransform());

            if (!IsShown)
                Show();

            await LayoutHelper.ForceRebuildLayoutsAsync(childs);

            Hide();

            returnToNormalCanvas.Dispose();
        }

        private async UniTask InitShowableAsync()
        {
            IDisposable returnToNormalCanvas = root.GetValue(this)
                .RectTransform()
                .MoveToDevCanvas();

            await WaitUntilChildrensInitedAsync();
            await RebuildControlledLayouts();
            await InitVisibleStateAsync(parent.IsSome);

            returnToNormalCanvas.Dispose();

            IsInited = true;
        }

        private sealed class HideCommand : PoolableCommand
        {
            public Maybe<GUITab> Tab { get; set; }

            public override bool IsReadyToExecute {
                get
                {
                    return base.IsReadyToExecute
                           &&
                           Tab.Map(target => target.IsInited).GetValue();
                }
            }

            public HideCommand()
                :
                base()
            {
            }

            public override void OnDespawned()
            {
                Tab = Maybe<GUITab>.None;

                base.OnDespawned();
            }

            public HideCommand SetGUITab(GUITab? guiTab)
            {
                Tab = guiTab;
                return this;
            }

            protected override void OnExecute()
            {
                Tab.IfSome(target =>
                {
                    target.OnHide();
                    target.DoHide();
                });
            }
        }

        private sealed class ShowCommand : PoolableCommand, IPoolable
        {
            public Maybe<GUITab> Tab { get; set; }

            public override bool IsReadyToExecute {
                get
                {
                    return base.IsReadyToExecute
                           &&
                           Tab.Map(target => target.IsInited).GetValue();
                }
            }

            public ShowCommand()
                :
                base()
            {
            }

            public override void OnDespawned()
            {
                Tab = Maybe<GUITab>.None;
                base.OnDespawned();
            }

            public ShowCommand SetGUITab(GUITab? guiTab)
            {
                Tab = guiTab;
                return this;
            }

            protected override void OnExecute()
            {
                Tab.IfSome(static target =>
                {
                    target.OnShow();
                    target.DoShow();
                });
            }
        }

        private sealed class RedrawCommand : PoolableCommandAsync
        {
            public Maybe<GUITab> Tab { get; set; }

            public override bool IsReadyToExecute {
                get
                {
                    return base.IsReadyToExecute
                           &&
                           Tab.Map(target => target.IsInited).GetValue();
                }
            }

            public RedrawCommand()
                :
                base()
            {
            }

            public override void OnDespawned()
            {
                Tab = Maybe<GUITab>.None;
                base.OnDespawned();
            }

            public RedrawCommand SetGUITab(GUITab? guiTab)
            {
                Tab = guiTab;
                return this;
            }

            protected override async ValueTask OnExecuteAsync(CancellationToken cancellationToken)
            {
                if (!Tab.TryGetValue(out var target))
                    return;

                await target.RebuildControlledLayouts();
            }
        }
    }
}
