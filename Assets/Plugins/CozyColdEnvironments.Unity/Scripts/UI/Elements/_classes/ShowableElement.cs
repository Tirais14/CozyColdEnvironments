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

        private readonly ReactiveProperty<PanelRenderer?> renderer = new();

        private bool isUIReloadBinded;
        private bool isRendererSetted;
        private bool isRootElementSetted;

        private IDisposable? parentShowableRootElementBinding;

        public PanelRenderer? Renderer {
            get
            {
                if (!didStart || !isRendererSetted)
                {
                    renderer.Value = GetComponentInParent<PanelRenderer>();
                    isRendererSetted = true;
                }

                return renderer.Value;
            }
        }

        public VisualElement? RootElement {
            get
            {
                if (!didStart || !isRootElementSetted)
                {
                    InitRenderer();

                    if (CCDebug<ShowableElement>.IsEnabled)
                    {
                        this.PrintLog(DebugMessageBuilder.CreatePooled()
                            .AddMessage("Init renderer early invoked")
                            .AddProperty("Showable", this)
                            .ToStringAndDispose()
                            );
                    }
                }

                return rootElement.Value;
            }
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
            InitRenderer();
            InitAsync().Forget(ex => this.PrintException(ex));
        }


        protected override void OnTransformParentChanged()
        {
            base.OnTransformParentChanged();
            DetachRenderer();
            InitRenderer();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DetachRenderer();
            rootElement.Dispose();
            renderer.Dispose();
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

        public void RegisterRendererChangedCallbackOnce(Action<PanelRenderer> action)
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

        public Observable<PanelRenderer?> ObserveRenderer() => renderer;

        protected override void HideCore()
        {
            if (RootElement is null ||
                RootElement.style.display == DisplayStyle.None)
                return;

            RootElement.style.display = DisplayStyle.None;
        }

        protected override void ShowCore()
        {
            if (RootElement is null ||
                RootElement.style.display == DisplayStyle.Flex)
                return;

            RootElement.style.display = DisplayStyle.Flex;
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

        //protected override ICommandBase GetHideCommand(CancellationToken cancellationToken)
        //{
        //    string cmdName = NameFactory.CreateFromCallerCached(
        //        this,
        //        nameof(Hide)
        //        );

        //    return Command.Builder.WithName(cmdName)
        //        .WithState(this)
        //        .Asynchronously()
        //        .WithExecuteAction(async static (@this, cancellationToken) =>
        //        {
        //            await UniTask.DelayFrame(
        //                1,
        //                delayTiming: PlayerLoopTiming.Update,
        //                cancellationToken: cancellationToken
        //                );

        //            @this.HideInternal();
        //            @this.IsShown = false;
        //        })
        //        .BuildPooled()
        //        .Value
        //        .WithCancellationToken(destroyCancellationToken);
        //}

        private void InitVisibleState()
        {
            ShowInternal();

            if (!ShowOnInited)
                HideInternal();
        }

        private void OnParentShowableRootElementChanged(VisualElement? parentRoot)
        {
            if (parentRoot is not null)
            {
                if (rootElement.Value is not null && rootElement.Value.parent == parentRoot)
                    return;

                if (visualTree == null)
                {
                    rootElement.Value = parentRoot.Q<VisualElement>(name);

                    if (rootElement.Value is null)
                    {
                        foreach (var children in parentRoot.Children())
                        {
                            if (children.name != "root")
                                continue;

                            rootElement.Value = children;
                            break;
                        }

                        if (rootElement.Value is null)
                            throw new InvalidOperationException($"Not found any root with names: {name}, root");
                    }
                }
                else
                {
                    rootElement.Value?.RemoveFromHierarchy();

                    rootElement.Value = visualTree.CloneTree();
                    parentRoot.Add(rootElement.Value);

                    if (CCDebug<ShowableElement>.IsEnabled)
                    {
                        this.PrintLog(DebugMessageBuilder.CreatePooled()
                            .AddMessage("Visual tree cloned to parent root")
                            .AddProperty("ParentRootElement", parentRoot)
                            .ToStringAndDispose());
                    }
                }

                if (rootElement.Value is not null)
                {
                    rootElement.Value.userData = new GameObjectReferenceContainer(gameObject);
                    InitVisibleState();
                }
            }
            else
                rootElement.Value = null;
        }

        private void OnUIReload(PanelRenderer _, VisualElement root)
        {
            rootElement.Value = root;
            rootElement.Value.userData = new GameObjectReferenceContainer(gameObject);
            InitVisibleState();
        }


        private async UniTask InitAsync()
        {
            destroyCancellationToken.ThrowIfCancellationRequested();

            try
            {
                await WaitUntilChildrensInitedAsync();
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

        private void InitRenderer()
        {
            if (isRootElementSetted)
                return;

            renderer.Value = GetComponentInParent<PanelRenderer>();
            isRendererSetted = true;
            isRootElementSetted = true;

            if (Renderer == null)
                return;

            if (Parent.IsNotNull() && Parent.Renderer == Renderer)
            {
                parentShowableRootElementBinding = Parent.ObserveRootElement()
                    .Subscribe(OnParentShowableRootElementChanged);
            }
            else
            {
                if (visualTree != null)
                    this.PrintWarning($"Showable is not child of a renderer. {nameof(VisualTree)} will be ignored");

                Renderer.RegisterUIReloadCallback(OnUIReload);
                isUIReloadBinded = true;
            }
        }

        private void DetachRenderer()
        {
            if (rootElement.Value is not null)
                rootElement.Value.userData = null;

            if (isUIReloadBinded && Renderer != null)
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

            rootElement.Value = null;
            isRootElementSetted = false;
            renderer.Value = null;
            isRendererSetted = false;
        }
    }
}
