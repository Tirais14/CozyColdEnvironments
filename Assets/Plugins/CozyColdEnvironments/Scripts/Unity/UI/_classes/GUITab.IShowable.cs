using CCEnvs.Collections;
using CCEnvs.Patterns.Commands;
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
using UnityEngine.UI;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public partial class GUITab : IShowable
    {
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

        private CommandAsync hideCmd = null!;
        private CommandAsync showCmd = null!;
        private CommandAsync redrawCmd = null!;

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

            hideCmd = new HideCommand(this);
            showCmd = new ShowCommand(this);
            redrawCmd = new RedrawCommand(this);
        }

        private void IShowableStart()
        {
            InitShowableAsync().Forget();
        }

        private void IShowableOnDestroy()
        {
            isShown.Dispose();
            hideCmd.Dispose();
            showCmd.Dispose();
            redrawCmd.Dispose();
        }

        public void Hide()
        {
            commandScheduler.Schedule(hideCmd.Reset());
        }

        public void Show()
        {
            commandScheduler.Schedule(showCmd.Reset());
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
            commandScheduler.Schedule(redrawCmd.Reset());
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
                            destroyCancellationToken.CheckCancellationRequestByInterval(ref iterationsPassed);

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
            await LayoutHelper.ForceRebuildLayoutsAsync(childs);
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

        private sealed class HideCommand : CommandAsync
        {
            private readonly GUITab guiTab;

            public override bool IsReadyToExecute => base.IsReadyToExecute && guiTab.IsInited;

            public HideCommand(GUITab guiTab)
                :
                base()
            {
                this.guiTab = guiTab;
            }

            protected override ValueTask OnExecuteAsync(CancellationToken cancellationToken)
            {
                guiTab.OnHide();
                guiTab.DoHide();

                return default;
            }
        }

        private sealed class ShowCommand : CommandAsync
        {
            private readonly GUITab guiTab;

            public override bool IsReadyToExecute => base.IsReadyToExecute && guiTab.IsInited;

            public ShowCommand(GUITab guiTab)
                :
                base(delayFrameCount: 1)
            {
                this.guiTab = guiTab;
            }

            protected override ValueTask OnExecuteAsync(CancellationToken cancellationToken)
            {
                guiTab.OnShow();
                guiTab.DoShow();

                return default;
            }
        }

        private sealed class RedrawCommand : CommandAsync
        {
            private readonly GUITab guiTab;

            public override bool IsReadyToExecute => base.IsReadyToExecute && guiTab.IsInited;

            public RedrawCommand(GUITab guiTab)
                :
                base(delayFrameCount: 2)
            {
                this.guiTab = guiTab;
            }

            protected override async ValueTask OnExecuteAsync(CancellationToken cancellationToken)
            {
                await guiTab.RebuildControlledLayouts();
            }
        }
    }
}
