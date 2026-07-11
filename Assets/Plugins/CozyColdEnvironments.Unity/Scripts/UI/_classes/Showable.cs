using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Patterns.Commands;
using CCEnvs.Pools;
using CCEnvs.Snapshots;
using CCEnvs.Threading;
using CCEnvs.UnityX.Async;
using CCEnvs.UnityX.Components;
using CCEnvs.UnityX.Injections;
using CCEnvs.UnityX.Snapshots.UI;
using Cysharp.Threading.Tasks;
using Humanizer;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

#nullable enable
namespace CCEnvs.UnityX.UI
{
    [DisallowMultipleComponent]
    public class Showable : ShowableBase<IShowable>, IShowable
    {
        [Header("Showable settings")]
        [Space(8)]

        [SerializeField]
        protected ShowableRenderMode renderMode;

        private PooledObject<List<IDisposable>> transparentGraphics;

        private readonly Lazy<Dictionary<object, ISnapshot>> snapshots = new(() => new Dictionary<object, ISnapshot>());

        public ShowableRenderMode RenderMode {
            get => renderMode;
            set => renderMode = value;
        }

        [field: GetBySelf(IsOptional = true)]
        public Graphic? graphic { get; private set; } = null!;

        public Image? image => graphic.As<Image>();

        [field: GetBySelf(IsOptional = true)]
        public CanvasGroup? canvasGroup { get; private set; }

        [field: GetByParent]
        public Canvas canvas { get; private set; } = null!;

        [field: GetByParent(IsOptional = true)]
        public ICanvasController? canvasController { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            InitCanvasGroup();
            SetGraphicsTransparent();
        }

        protected override void Start()
        {
            base.Start();
            SetCanvasController();
            InitShowableAsync().ForgetByPrintException();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            transparentGraphics.Dispose();
        }

        public override void Redraw()
        {
            ThrowIfInitFailured();

            if (!IsEnabled)
                return;

            Command.Builder.WithName(nameof(Redraw), this)
                .WithState(this)
                .Asynchronously()
                .WithExecuteAction(
                static async (@this, cancellationToken) =>
                {
                    await @this.RebuildControlledLayouts(cancellationToken, initCall: false);
                })
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(destroyCancellationToken)
                .ScheduleBy(commandScheduler);
        }

        protected override void HideCore()
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

        protected override void ShowCore()
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

        protected virtual void OnInited()
        {
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
            if (transparentGraphics == default
                ||
                transparentGraphics.Value is null)
            {
                return;
            }

            transparentGraphics.Value.DisposeEachAndClear();
            transparentGraphics.Dispose();
        }

        private async UniTask InitShowableAsync()
        {
            destroyCancellationToken.ThrowIfCancellationRequested();

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

                await InitVisibleStateAsync();

                OnInited();

                isInitedCmd?.Execute(true);

                commandScheduler.Enable();
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

        private void SetCanvasController()
        {
            canvasController = this.Q()
                .FromParents()
                .Component<ICanvasController>().Raw;
        }

        private ICommandBase GetHideCommand(CancellationToken cancellationToken)
        {
            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(Hide),
                expirationTimeRelativeToNow: 5.Minutes()
                );

            return Command.Builder.WithName(cmdName)
                .WithState(this)
                .Synchronously()
                .WithExecuteAction(
                static @this => @this.HideInternal())
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken);
        }

        private ICommandBase GetShowCommand(CancellationToken cancellationToken)
        {
            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(Hide),
                expirationTimeRelativeToNow: 5.Minutes()
                );

            return Command.Builder.WithName(cmdName)
                .WithState(this)
                .WithExecutePredicate(
                static @this =>
                {
                    return @this.IsReadyToShow;
                })
                .Synchronously()
                .WithExecuteAction(
                static @this => @this.ShowInternal())
                .BuildPooled()
                .Value
                .AttachExternalCancellationToken(cancellationToken);
        }
    }
}
