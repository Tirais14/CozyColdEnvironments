using CCEnvs.Collections;
using CCEnvs.Patterns.Commands;
using CCEnvs.Snapshots;
using CCEnvs.Unity.Components;
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

        private Command hideCmd = null!;
        private Command onHidenCmd = null!;
        private Command showCmd = null!;
        private Command onShownCmd = null!;
        private Command redrawCmd = null!;

        public bool ShowOnInited {
            get => m_ShowOnInited;
            set => m_ShowOnInited = value;
        }

        public bool IsShown => isShown.Value;
        public virtual bool ShowAllowed => IsInited && !stateTransitioning;
        public virtual bool HideAllowed => IsInited && !stateTransitioning;
        public bool IsInited { get; private set; }

        private void IShowableAwake()
        {
            if (canvasGroup.IsNone
                &&
                showableRenderMode == ShowableRenderMode.CanvasGroup)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            CreateShowCommands();
            CreateHideCommands();
            CreateRedrawCommand();
        }

        private void IShowableStart()
        {
            InitShowableAsync().Forget();
        }

        private void IShowableOnDestroy()
        {
            isShown.Dispose();
            hideCmd.Dispose();
            onHidenCmd.Dispose();
            showCmd.Dispose();
            onShownCmd.Dispose();
            redrawCmd.Dispose();
        }

        public virtual void Hide()
        {
            commandScheduler.Schedule(hideCmd.Reset());
            commandScheduler.Schedule(onHidenCmd.Reset());
        }

        public virtual void Show()
        {
            commandScheduler.Schedule(showCmd.Reset());
            commandScheduler.Schedule(onShownCmd.Reset());
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

        private async UniTask WaitUntilChildrensInitedAsync(IShowable[] childs)
        {
            if (ArrayHelper.IsNotEmpty(childs))
            {
                await UniTask.WaitUntil(
                    childs,
                    childs => childs.All(x => x.IsInited),
                    cancellationToken: destroyCancellationToken
                    );
            }
        }

        private void CreateHideCommands()
        {
            hideCmd = new HideCommand(this);
            onHidenCmd = new OnHidenCommand(this);
        }

        private void CreateShowCommands()
        {
            showCmd = new ShowCommand(this);
            onShownCmd = new OnShownCommand(this);
        }

        private void CreateRedrawCommand()
        {
            redrawCmd = new RedrawCommand(this);
        }

        private IShowable[] GetShowableChilds()
        {
            return this.Q()
                .FromChildrens()
                .ExcludeSelf()
                .Components<IShowable>()
                .Where(x => !x.IsInited)
                .ToArray();
        }

        private (RectTransform child, Vector3 pos)[] GetChildsForRebuildLayout()
        {
            using var _ = UnityEngine.Pool.ListPool<(RectTransform child, Vector3 pos)>.Get(out var childInfos);

            foreach (var child in GetComponentsInChildren<RectTransform>(includeInactive: false))
            {
                if (child.Q()
                    .FromParents()
                    .ExcludeSelf()
                    .Component<IShowable>()
                    .Lax()
                    .TryGetValue(out var parentShowable)
                    &&
                    !parentShowable.Equals(this))
                {
                    continue;
                }

                childInfos.Add((child, child.localPosition));
            }

            return childInfos.ToArray();
        }

        private async UniTask RebuildLayoutAsync((RectTransform child, Vector3 pos)[] childInfos)
        {
            await UniTask.NextFrame();
            await UniTask.WaitForEndOfFrame();

            foreach (var childInfo in childInfos)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(childInfo.child);

                childInfo.child.localPosition = childInfo.pos;
            }
        }

        private async UniTask InitShowableAsync()
        {
            var showableChilds = GetShowableChilds();
            var childsForRebuildLayout = GetChildsForRebuildLayout();

            var returnToNormalCanvas = this.RectTransform().MoveToDevCanvas();

            await WaitUntilChildrensInitedAsync(showableChilds);
            await RebuildLayoutAsync(childsForRebuildLayout);
            await InitVisibleStateAsync(parent.IsSome);

            returnToNormalCanvas.Dispose();

            IsInited = true;
        }

        private sealed class HideCommand : Command
        {
            private readonly GUITab guiTab;

            public override bool IsReadyToExecute => base.IsReadyToExecute && guiTab.HideAllowed;

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

        private sealed class OnHidenCommand : Command
        {
            private readonly GUITab guiTab;

            public OnHidenCommand(GUITab guiTab)
                :
                base()
            {
                this.guiTab = guiTab;
            }

            protected override ValueTask OnExecuteAsync(CancellationToken cancellationToken)
            {
                guiTab.OnHiden();

                return default;
            }
        }

        private sealed class ShowCommand : Command
        {
            private readonly GUITab guiTab;

            public override bool IsReadyToExecute => base.IsReadyToExecute && guiTab.ShowAllowed;

            public ShowCommand(GUITab guiTab)
                :
                base()
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

        private sealed class OnShownCommand : Command
        {
            private readonly GUITab guiTab;

            public OnShownCommand(GUITab guiTab)
                :
                base()
            {
                this.guiTab = guiTab;
            }

            protected override ValueTask OnExecuteAsync(CancellationToken cancellationToken)
            {
                guiTab.OnShown();

                return default;
            }
        }

        private sealed class RedrawCommand : Command
        {
            private readonly GUITab guiTab;

            public RedrawCommand(GUITab guiTab)
                :
                base()
            {
                this.guiTab = guiTab;
            }

            protected override async ValueTask OnExecuteAsync(CancellationToken cancellationToken)
            {
                var toRebuild = guiTab.GetChildsForRebuildLayout();

                var childShowables = guiTab.Q()
                    .FromChildrens()
                    .ExcludeSelf()
                    .FirstComponentsOnBranch()
                    .Components<IShowable>();

                foreach (var item in childShowables)
                {
                    item.Redraw();
                }

                await guiTab.RebuildLayoutAsync(toRebuild);

                if (guiTab.IsShown)
                {
                    guiTab.Hide();
                    guiTab.Show();
                }
            }
        }
    }
}
