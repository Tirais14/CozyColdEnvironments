using CCEnvs.Collections;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
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

        private PooledHandle<List<(Graphic graphic, GraphicSnapshot snapshot)>> initHidenGraphics;

        public bool ShowOnInited {
            get => m_ShowOnInited;
            set => m_ShowOnInited = value;
        }

        public bool IsShown => isShown.Value;
        public bool IsInited { get; private set; }
        public virtual bool IsReadyToShow => true;

        protected bool isLayoutsRebuilding { get; private set; }
        protected bool isInitFaulted { get; private set; }

        private void IShowableAwake()
        {
            commandScheduler.Disable();

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

            ValidateInit();

            var linkedTokenSource = destroyCancellationToken.LinkTokens(cancellationToken);

            cancellationToken = linkedTokenSource.Token;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

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
            ValidateInit();

            Command.Builder.SetName(nameof(Hide), this)
                .WithState(this)
                .Syncronously()
                .SetExecuteAction(
                static @this => @this.HideInternal())
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

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

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
            ValidateInit();

            Command.Builder.SetName(nameof(Show), this)
                .WithState(this)
                .SetExecutePredicate(
                static @this =>
                {
                    return @this.IsReadyToShow;
                })
                .Syncronously()
                .SetExecuteAction(
                static @this => @this.ShowInternal())
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

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

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
            ValidateInit();

            Command.Builder.SetName(nameof(Redraw), this)
                .WithState(this)
                .Asyncronously()
                .SetExecuteAction(
                static async (@this, cancellationToken) =>
                {
                    await @this.RebuildControlledLayouts(cancellationToken, initCall: false);
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(destroyCancellationToken)
                .ScheduleBy(commandScheduler);
        }

        public async UniTask RedrawAsync(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = destroyCancellationToken.LinkTokens(cancellationToken);

            cancellationToken = linkedTokenSource.Token;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                Redraw();

                await UniTask.WaitWhile(
                    this,
                    static @this => @this.isLayoutsRebuilding,
                    cancellationToken: cancellationToken
                    );
            }
            finally
            {
                linkedTokenSource.Dispose();
            }
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

        protected virtual void HideCore()
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

        protected virtual void ShowCore()
        {
            switch (showableRenderMode)
            {
                case ShowableRenderMode.GameObject:
                    {
                        gameObject.SetActive(true);
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

                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        protected virtual void OnShown()
        {
        }

        protected InvalidOperationException GetInitFaultedException()
        {
            return new InvalidOperationException($"{nameof(GUITab)}: {this} is not correctly initialized");
        }

        protected void ValidateInit()
        {
            if (isInitFaulted)
                throw GetInitFaultedException();
        }


        protected void ShowInternal()
        {
            OnShow();
            ShowCore();
            SetShown();
            OnShown();
        }

        protected void HideInternal()
        {
            OnHide();
            HideCore();
            SetHiden();
            OnHiden();
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
                ShowInternal();
            else
                HideInternal();
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

        private async UniTask RebuildControlledLayouts(CancellationToken cancellationToken, bool initCall)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var childs = GetControlledActiveLayouts();

            var returnToNormalCanvas = CanvasHelper.MoveToDevCanvas(this.RectTransform());

            bool isShown = IsShown;

            isLayoutsRebuilding = true;

            try
            {
                if (!isShown)
                {
                    if (initCall)
                        ShowInternal();
                    else
                        await ShowAsync(cancellationToken);
                }

                await LayoutHelper.ForceRebuildLayoutsAsync(childs, cancellationToken);

                if (!isShown)
                {
                    if (initCall)
                        HideInternal();
                    else
                        await HideAsync(cancellationToken);
                }
            }
            finally
            {
                isLayoutsRebuilding = false;

                returnToNormalCanvas.Dispose();
            }
        }

        private PooledHandle<List<(Graphic graphic, GraphicSnapshot snapshot)>> SetGraphicsTransparent()
        {
            var graphics = this.Q().FromChildrens().DepthLimiter<IShowable>().Components<Graphic>();

            var results = ListPool<(Graphic graphic, GraphicSnapshot snapshot)>.Shared.Get();

            GraphicSnapshot snapshot;

            foreach (var graphic in graphics)
            {
                snapshot = new GraphicSnapshot(graphic);

                graphic.color = graphic.color.WithAlpha(0.001f);

                results.Value.Add((graphic, snapshot));
            }

            return results;
        }

        private async UniTask InitShowableAsync()
        {
            //foreach (var hidenGraphic in initHidenGraphics.Value)
            //    hidenGraphic.snapshot.TryRestore(hidenGraphic.graphic, out _);

            //initHidenGraphics.Dispose();

            destroyCancellationToken.ThrowIfCancellationRequested();

            IDisposable returnToNormalCanvas = root.GetValue(this)
                .RectTransform()
                .MoveToDevCanvas();

            var layoutGroup = this.Q()
                .FromParents()
                .ExcludeSelf()
                .Component<LayoutGroup>()
                .Lax()
                .Where(static layout => layout.enabled);

            try
            {
                await WaitUntilChildrensInitedAsync();

                await RebuildControlledLayouts(destroyCancellationToken, initCall: true);

                await InitVisibleStateAsync(parent.IsSome);
            }
            catch (Exception)
            {
                isInitFaulted = true;
                throw;
            }
            finally
            {
                IsInited = true;

                layoutGroup.IfSome(static layout => layout.enabled = true);

                returnToNormalCanvas.Dispose();
            }

            commandScheduler.Enable();
        }
    }
}
