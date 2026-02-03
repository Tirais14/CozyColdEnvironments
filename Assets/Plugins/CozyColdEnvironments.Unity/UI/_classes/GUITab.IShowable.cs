using CCEnvs.Collections;
using CCEnvs.Patterns.Commands;
using CCEnvs.Snapshots;
using CCEnvs.Unity.Async;
using CCEnvs.Unity.Snapshots.UI;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
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

        private bool isLayoutsRebuilding;

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
            InitShowableAsync().ForgetByPrintException();
        }

        private void IShowableOnDestroy()
        {
            isShown.Dispose();
        }

        public async UniTask WaitForInitializedAsync(CancellationToken cancellationToken = default)
        {
            if (IsInited)
                return;

            var linkedTokenSource = destroyCancellationToken.LinkTokens(cancellationToken);

            cancellationToken = linkedTokenSource.Token;

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await UniTask.WaitUntil(
                    this,
                    static @this => @this.IsInited,
                    cancellationToken: cancellationToken
                    );
            }
            finally
            {
                linkedTokenSource.Dispose();
            }
        }

        public void Hide()
        {
            Command.Builder.SetName(nameof(Hide), this)
                .WithState(this)
                .Syncronously()
                .SetExecuteAction(
                static @this =>
                {
                    @this.OnHide();
                    @this.DoHide();
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(destroyCancellationToken)
                .ScheduleBy(commandScheduler);
        }

        public async UniTask HideAsync(CancellationToken cancellationToken = default)
        {
            if (!IsShown)
                return;

            var linkedTokenSource = destroyCancellationToken.LinkTokens(cancellationToken);

            cancellationToken = linkedTokenSource.Token;

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                Hide();

                await UniTask.WaitWhile(
                    this,
                    static @this => @this.IsShown,
                    cancellationToken: cancellationToken
                    );
            }
            finally
            {
                linkedTokenSource.Dispose();
            }
        }

        public void Show()
        {
            Command.Builder.SetName(nameof(Show), this)
                .WithState(this)
                .Syncronously()
                .SetExecuteAction(
                static @this =>
                {
                    @this.OnShow();
                    @this.DoShow();
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(destroyCancellationToken)
                .ScheduleBy(commandScheduler);
        }

        public async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            if (IsShown)
                return;

            var linkedTokenSource = destroyCancellationToken.LinkTokens(cancellationToken);

            cancellationToken = linkedTokenSource.Token;

            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                Show();

                await UniTask.WaitUntil(
                    this,
                    static @this => @this.IsShown,
                    cancellationToken: cancellationToken
                    );
            }
            finally
            {
                linkedTokenSource.Dispose();
            }
        }

        public bool SwitchShownState()
        {
            if (IsShown)
            {
                Hide();
                return false;
            }
            else
            {
                Show();
                return true;
            }
        }

        public async UniTask<bool> SwitchShownStateAsync(CancellationToken cancellationToken = default)
        {
            if (IsShown)
                await HideAsync();
            else
                await ShowAsync();

            return IsShown;
        }

        public void SwitchShownStateVoid() => SwitchShownState();

        public void Redraw()
        {
            Command.Builder.SetName(nameof(Redraw), this)
                .WithState(this)
                .Asyncronously()
                .SetExecuteAction(
                static async (@this, cancellationToken) =>
                {
                    await @this.RebuildControlledLayouts(cancellationToken);
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(destroyCancellationToken)
                .ScheduleBy(commandScheduler);
        }

        public async UniTask RedrawAsync(CancellationToken cancellationToken = default)
        {
            
        }

        public Observable<bool> ObserveShow()
        {
            return isShown.Where(static x => x);
        }

        public Observable<bool> ObserveHide()
        {
            return isShown.Where(static x => !x);
        }

        public Observable<bool> ObserveIsInited()
        {
            throw new NotImplementedException();
        }

        protected virtual void OnHide()
        {
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
            isShown.Value = true;

            OnShown();
        }

        private void SetHiden()
        {
            isShown.Value = false;

            OnHiden();
        }

        private async UniTask InitVisibleStateAsync(bool hasParent)
        {
            destroyCancellationToken.ThrowIfCancellationRequested();

            if (ShowOnInited)
            {
                Show();

                await UniTask.WaitUntil(
                    this,
                    static @this => @this.IsShown,
                    cancellationToken: destroyCancellationToken
                    );
            }
            else if (!hasParent)
            {
                Hide();

                await UniTask.WaitWhile(
                    this,
                    static @this => @this.IsShown,
                    cancellationToken: destroyCancellationToken
                    );
            }
        }

        private async UniTask WaitUntilChildrensInitedAsync()
        {
            destroyCancellationToken.ThrowIfCancellationRequested();

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

        private RectTransform[] GetControlledActiveLayouts()
        {
            return this.Q()
                .FromChildrens()
                .DepthLimiter<IShowable>()
                .Components<RectTransform>()
                .ToArray();
        }

        private async UniTask RebuildControlledLayouts(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var childs = GetControlledActiveLayouts();

            var returnToNormalCanvas = CanvasHelper.MoveToDevCanvas(this.RectTransform());

            bool isShown = IsShown;

            isLayoutsRebuilding = true;

            try
            {
                if (!isShown || !IsInited)
                    await ShowAsync(cancellationToken);

                await LayoutHelper.ForceRebuildLayoutsAsync(childs, cancellationToken);

                if (!isShown || !IsInited)
                    await HideAsync(cancellationToken);
            }
            finally
            {
                returnToNormalCanvas.Dispose();
                isLayoutsRebuilding = false;
            }
        }

        private async UniTask InitShowableAsync()
        {
            destroyCancellationToken.ThrowIfCancellationRequested();

            IDisposable returnToNormalCanvas = root.GetValue(this)
                .RectTransform()
                .MoveToDevCanvas();

            var hasParent = parent.IsSome;

            try
            {
                await WaitUntilChildrensInitedAsync();

                await RebuildControlledLayouts(destroyCancellationToken);

                await InitVisibleStateAsync(parent.IsSome);
            }
            finally
            {
                returnToNormalCanvas.Dispose();
            }

            IsInited = true;
        }
    }
}
