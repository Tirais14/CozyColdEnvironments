#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Disposables;
using CCEnvs.Patterns.Commands;
using CCEnvs.UnityX.ComponentInjections;
using CommunityToolkit.Diagnostics;
using Cysharp.Threading.Tasks;
using R3;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

namespace CCEnvs.UnityX.UI.Elements
{
    [DisallowMultipleComponent]
    public class ShowableElement
        :
        ShowableBase<IShowableElement>,
        IShowableElement
    {
        [SerializeField]
        protected VisualTreeAsset? visualTree;

        [SerializeField, Min(0f)]
        protected int showCommandDelayFramCount = 1;

        private readonly ReactiveProperty<VisualElement?> rootElement = new();

        [GetByParent]
        private PanelRenderer renderer = null!;

        private bool isUIReloadBinded;

        private IDisposable? parentShowableRootElementBinding;

        public PanelRenderer Renderer {
            get
            {
                if (renderer == null)
                {
                    renderer = this.Q()
                        .FromParents()
                        .IncludeInactive()
                        .Component<PanelRenderer>()
                        .Strict();
                }

                return renderer;
            }
        }

        public VisualElement? RootElement {
            get => rootElement.Value;
            private set
            {
                rootElement.Value = value;
            }
        }

        public int ShowCommandDelayFrameCount {
            get => showCommandDelayFramCount;
            set => SetShowCommandDelayFrameCount(value);
        }

        public VisualTreeAsset? VisualTree {
            get => visualTree;
            set => SetVisualTree(value);
        }

        protected override void Start()
        {
            base.Start();
            InitAsync().Forget(ex => this.PrintException(ex));
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (Parent.IsNotNull() && Parent.Renderer == Renderer)
            {
                parentShowableRootElementBinding = Parent.ObserveRootElement()
                    .Subscribe(OnParentShowableRootElementChanged);
            }
            else
            {
                if (CCDebug<ShowableElement>.IsEnabled && visualTree != null)
                    this.PrintWarning($"Showable is not child of a renderer. {nameof(VisualTree)} will be ignored");

                Renderer.RegisterUIReloadCallback(OnUIReload);
                isUIReloadBinded = true;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (RootElement is not null)
                RootElement.userData = null;

            if (isUIReloadBinded)
            {
                Renderer.UnregisterUIReloadCallback(OnUIReload);
                isUIReloadBinded = false;
            }
            else
            {
                CCDisposable.Dispose(ref parentShowableRootElementBinding);

                if (visualTree != null)
                    RootElement?.RemoveFromHierarchy();
            }

            RootElement = null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            rootElement.Dispose();
        }

        public ShowableElement SetShowCommandDelayFrameCount(int value)
        {
            showCommandDelayFramCount = Math.Max(value, 0);
            return this;
        }

        public ShowableElement SetVisualTree(VisualTreeAsset? value)
        {
            visualTree = value;
            return this;
        }

        public void RegisterRendererChagnedCallbackOnce(Action<PanelRenderer> action)
        {
            Guard.IsNotNull(action);

            if (Renderer != null)
                action(Renderer);
        }

        public override void Redraw()
        {
            HideCore();
            ShowCore();
        }

        public Observable<VisualElement?> ObserveRootElement() => rootElement;

        protected override void HideCore()
        {
            if (RootElement is null)
                return;

            RootElement.style.display = DisplayStyle.None;

            if (CCDebug<ShowableElement>.IsEnabled)
            {
                this.PrintLog(DebugMessageBuilder.CreatePooled()
                    .AddMessage("Root state changed")
                    .AddProperty(nameof(RootElement), RootElement)
                    .AddProperty(nameof(RootElement.visible), RootElement.visible)
                    .ToStringAndDispose()
                    );
            }
        }

        protected override void ShowCore()
        {
            if (RootElement is null)
                return;

            RootElement.style.display = DisplayStyle.Flex;

            if (CCDebug<ShowableElement>.IsEnabled)
            {
                this.PrintLog(DebugMessageBuilder.CreatePooled()
                    .AddMessage("Root state changed")
                    .AddProperty(nameof(RootElement), RootElement)
                    .AddProperty(nameof(RootElement.visible), RootElement.visible)
                    .ToStringAndDispose()
                    );
            }
        }

        protected virtual void OnInited() { }

        protected override ICommandBase GetShowCommand(CancellationToken cancellationToken)
        {
            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(Show)
                );

            return Command.Builder.WithName(cmdName)
                .WithState(this)
                .WithExecutePredicate(@this => @this.IsReadyToShow)
                .Asynchronously()
                .WithExecuteAction(async (@this, cancellationToken) =>
                {
                    if (@this.showCommandDelayFramCount >= 1)
                    {
                        await UniTask.DelayFrame(
                            delayFrameCount: @this.showCommandDelayFramCount,
                            delayTiming: PlayerLoopTiming.Update,
                            cancellationToken: cancellationToken
                            );
                    }

                    @this.ShowInternal();
                    @this.IsShown = true;
                    @this.OnShown();
                })
                .BuildPooled()
                .Value
                .WithCancellationToken(destroyCancellationToken);
        }

        private void OnParentShowableRootElementChanged(VisualElement? parentRoot)
        {
            if (parentRoot is not null)
            {
                if (RootElement is not null && RootElement.parent == parentRoot)
                    return;

                if (visualTree == null)
                    RootElement = parentRoot.Q<VisualElement>(name);
                else
                {
                    RootElement?.RemoveFromHierarchy();

                    RootElement = visualTree.CloneTree();
                    parentRoot.Add(RootElement);

                    if (CCDebug<ShowableElement>.IsEnabled)
                    {
                        this.PrintLog(DebugMessageBuilder.CreatePooled()
                            .AddMessage("Visual tree cloned to parent root")
                            .AddProperty("ParentRootElement", parentRoot)
                            .ToStringAndDispose());
                    }
                }

                if (RootElement is not null)
                    RootElement.userData = new GameObjectReferenceContainer(gameObject);
            }
            else
            {
                RootElement = null;
            }
        }

        private void OnUIReload(PanelRenderer _, VisualElement root)
        {
            RootElement = root;
            RootElement.userData = new GameObjectReferenceContainer(gameObject);
        }

        private async UniTask InitAsync()
        {
            destroyCancellationToken.ThrowIfCancellationRequested();

            try
            {
                await WaitUntilChildrensInitedAsync();
                await InitVisibleStateAsync();
                OnInited();
                ExecuteOnInitedEvent();
                commandScheduler.Enable();
            }
            catch (System.Exception)
            {
                isInitFaulted = true;
                throw;
            }
            finally
            {
                IsInited = true;
            }
        }
    }
}
