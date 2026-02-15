using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using CCEnvs.Snapshots;
using CCEnvs.Threading;
using CCEnvs.Unity.Async;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.Snapshots.UI;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.UI
{
    [DisallowMultipleComponent]
    public class Showable : CCBehaviour, IShowable
    {
        [Header("Showable settings")]
        [Space(8)]

        [SerializeField]
        protected ShowableRenderMode renderMode;

        [SerializeField]
        protected bool showOnInited;

        [SerializeField]
        protected bool preventHide;

        [SerializeField]
        protected bool isEnabled = true;

        private readonly ReactiveProperty<bool> isShown = new(true);

        private readonly Lazy<Dictionary<object, ISnapshot>> snapshots = new(() => new Dictionary<object, ISnapshot>());

        private readonly CommandScheduler commandScheduler = new(UnityFrameProvider.Update, nameof(Showable));

        private ReactiveCommand<bool>? isInitedCmd;

        private MonoBehaviour? _root;
        private MonoBehaviour? _parent;

        private PooledObject<List<IDisposable>> transparentGraphics;

        public bool ShowOnInited {
            get => showOnInited;
            set => showOnInited = value;
        }

        public bool IsShown => isShown.Value;
        public bool IsInited { get; private set; }
        public virtual bool IsReadyToShow => IsEnabled;

        public bool IsEnabled {
            get => isEnabled;
            set => isEnabled = value;
        }

        public bool PreventHide {
            get => preventHide;
            set => preventHide = value;
        }

        [field: GetBySelf(IsOptional = true)]
        public Graphic? graphic { get; private set; } = null!;

        [field: GetBySelf(IsOptional = true)]
        public Image? image { get; private set; } = null!;

        [field: GetBySelf(IsOptional = true)]
        public CanvasGroup? canvasGroup { get; private set; }

        [field: GetByParent]
        public Canvas canvas { get; private set; } = null!;

        public IShowable? root => (IShowable?)_root;
        public IShowable? parent => (IShowable?)_parent;

        [field: GetByParent(IsOptional = true)]
        public ICanvasController? canvasController { get; private set; }

        public ShowableRenderMode RenderMode {
            get => renderMode;
            set => renderMode = value;
        }

        protected bool isLayoutsRebuilding { get; private set; }
        protected bool isInitFaulted { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            commandScheduler.Disable(); //disabling until IsInited
            InitCanvasGroup();
            ObserveTransformParent();
            SetGraphicsTransparent();
        }

        protected override void Start()
        {
            base.Start();
            SetRoot();
            SetParent();
            SetCanvasController();
            InitShowableAsync().ForgetByPrintException();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            commandScheduler.Dispose();
            isShown.Dispose();
            transparentGraphics.Dispose();
        }

        public async UniTask WaitUntilInited(CancellationToken cancellationToken = default)
        {
            if (IsInited)
                return;

            ValidateInit();

            using var _ = destroyCancellationToken.TryLinkTokens(
                cancellationToken,
                out cancellationToken
                );

            cancellationToken.ThrowIfCancellationRequested();

            await UniTask.WaitUntil(
                this,
                static @this => @this.IsInited,
                cancellationToken: cancellationToken
                );
        }

        public void Hide()
        {
            ValidateInit();

            if (!IsEnabled || PreventHide)
                return;

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
            if (!IsShown || PreventHide)
                return;

            ValidateInit();

            if (!IsEnabled)
                return;

            using var _ = destroyCancellationToken.TryLinkTokens(
                cancellationToken,
                out cancellationToken
                );

            cancellationToken.ThrowIfCancellationRequested();

            Hide();

            await UniTask.WaitWhile(
                this,
                static @this => @this.IsShown,
                cancellationToken: cancellationToken
                );
        }

        public void Show()
        {
            ValidateInit();

            if (!IsEnabled)
                return;

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

            ValidateInit();

            if (!IsEnabled)
                return;

            using var _ = destroyCancellationToken.TryLinkTokens(
                cancellationToken, 
                out cancellationToken
                );

            cancellationToken.ThrowIfCancellationRequested();

            Show();

            await UniTask.WaitUntil(
                this,
                static @this => @this.IsShown,
                cancellationToken: cancellationToken
                );
        }

        public bool SwitchShownState()
        {
            ValidateInit();

            if (!IsEnabled)
                return IsShown;

            if (IsShown)
                Hide();
            else
                Show();

            return IsShown;
        }

        public async UniTask<bool> SwitchShownStateAsync(CancellationToken cancellationToken = default)
        {
            if (!IsEnabled)
                return IsShown;

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

            if (!IsEnabled)
                return;

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
            ValidateInit();

            if (!IsEnabled)
                return;

            using var _ = destroyCancellationToken.TryLinkTokens(
                cancellationToken,
                out cancellationToken
                );

            cancellationToken.ThrowIfCancellationRequested();

            Redraw();

            await UniTask.WaitWhile(
                this,
                static @this => @this.isLayoutsRebuilding,
                cancellationToken: cancellationToken
                );
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
            isInitedCmd ??= new ReactiveCommand<bool>();

            return isInitedCmd;
        }

        public IShowable[] GetDirectChilds()
        {
            return this.Q()
                .FromChildrens()
                .ExcludeSelf()
                .FirstComponentsOnBranch()
                .Components<IShowable>()
                .ToArray();
        }

        public T[] GetChilds<T>()
        {
            return this.Q()
                .FromChildrens()
                .DepthLimiter<IShowable>()
                .Components<T>()
                .ToArray();
        }

        protected virtual void OnHide()
        {
        }

        protected virtual void HideCore()
        {
            switch (renderMode)
            {
                case ShowableRenderMode.GameObject:
                    {
                        gameObject.SetActive(false);
                        SetHiden();
                    }
                    break;

                case ShowableRenderMode.CanvasGroup:
                    {
                        CC.Guard.IsNotNull(canvasGroup, nameof(canvasGroup));

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
            switch (renderMode)
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

        protected virtual void OnInited()
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

        private async UniTask InitVisibleStateAsync()
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

        private async UniTask RebuildControlledLayouts(CancellationToken cancellationToken, bool initCall)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var childs = GetChilds<RectTransform>();

            var returnToNormalCanvas = this.RectTransform().MoveToDevCanvas();

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

        private void SetGraphicsTransparent()
        {
            UnsetGraphicsTransparent();

            var graphics = GetChilds<Graphic>();

            transparentGraphics = ListPool<IDisposable>.Shared.Get();

            foreach (var graphic in graphics)
                transparentGraphics.Value.Add(graphic.DoTransparent());
        }

        private void UnsetGraphicsTransparent()
        {
            transparentGraphics.Value.DisposeEachAndClear();
            transparentGraphics.Dispose();
        }

        private async UniTask InitShowableAsync()
        {
            destroyCancellationToken.ThrowIfCancellationRequested();

            IDisposable devCanvasHandle = _root.Maybe().GetValue(this)
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

                UnsetGraphicsTransparent();

                await RebuildControlledLayouts(destroyCancellationToken, initCall: true);

                await InitVisibleStateAsync();

                OnInited();

                isInitedCmd?.Execute(true);

                commandScheduler.Enable();

                devCanvasHandle.Dispose();
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
            }
        }

        private void InitCanvasGroup()
        {
            if (canvasGroup == null
                &&
                renderMode == ShowableRenderMode.CanvasGroup)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void SetRoot()
        {
            _root = this.Q()
                .FromParents()
                .ExcludeSelf()
                .Component<IShowable>()
                .Lax()
                .Cast<MonoBehaviour>()
                .RightTarget;
        }

        private void SetParent()
        {
            _parent = transform.root.Maybe()
                .Map(static trRoot =>
                {
                    return trRoot.Q()
                        .FromChildrens()
                        .Component<IShowable>()
                        .Lax()
                        .Cast<MonoBehaviour>()
                        .RightTarget;
                })
                .GetValue();
        }

        private void SetCanvasController()
        {
            canvasController = this.Q()
                .FromParents()
                .Component<ICanvasController>().Raw;
        }

        private void ObserveTransformParent()
        {
            Observable.EveryValueChanged(cTransform,
                static transform =>
                {
                    return transform.parent;
                })
                .Subscribe(this,
                static (_, @this) =>
                {
                    @this.SetParent();
                    @this.SetRoot();
                })
                .RegisterTo(destroyCancellationToken);
        }
    }
}
