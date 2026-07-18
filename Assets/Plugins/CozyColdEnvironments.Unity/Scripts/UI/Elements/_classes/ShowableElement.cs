#nullable enable
using CCEnvs.Diagnostics;
using CCEnvs.Patterns.Commands;
using CCEnvs.UnityX.Injections;
using Cysharp.Threading.Tasks;
using Humanizer;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

namespace CCEnvs.UnityX.UI.Elements
{
    [RequireComponent(typeof(PanelRenderer))]
    public class ShowableElement
        :
        ShowableBase<IShowableElement>,
        IShowableElement
    {
        private bool isUIReloadBinded;

        [field: GetBySelf]
        public PanelRenderer renderer { get; private set; } = null!;

        public VisualElement? rendererRoot { get; private set; }

        //public override bool IsShown {
        //    get => base.IsShown && (rendererRoot?.visible ?? false);
        //    protected set => base.IsShown = value;
        //}

        protected override void Start()
        {
            base.Start();
            InitAsync().Forget(ex => this.PrintException(ex));
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            renderer.RegisterUIReloadCallback(OnUIReload);
            isUIReloadBinded = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            renderer.UnregisterUIReloadCallback(OnUIReload);
            isUIReloadBinded = false;
        }

        public override void Redraw()
        {
            if (!isUIReloadBinded)
                return;

            HideCore();
            renderer.UnregisterUIReloadCallback(OnUIReload);
            renderer.RegisterUIReloadCallback(OnUIReload);
            ShowCore();
        }

        protected override void HideCore()
        {
            if (rendererRoot is null)
                return;

            rendererRoot.visible = false;

            if (CCDebug<ShowableElement>.IsEnabled)
            {
                this.PrintLog(DebugMessageBuilder.CreatePooled()
                    .AddMessage("Root state changed")
                    .AddProperty(nameof(rendererRoot.visible), rendererRoot.visible)
                    .ToStringAndDispose()
                    );
            }
        }

        protected override void ShowCore()
        {
            if (rendererRoot is null)
                return;

            rendererRoot.visible = true;


            if (CCDebug<ShowableElement>.IsEnabled)
            {
                this.PrintLog(DebugMessageBuilder.CreatePooled()
                    .AddMessage("Root state changed")
                    .AddProperty(nameof(rendererRoot.visible), rendererRoot.visible)
                    .ToStringAndDispose()
                    );
            }
        }

        protected virtual void OnInited() { }

        protected override ICommandBase GetShowCommand(CancellationToken cancellationToken)
        {
            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(Show),
                expirationTimeRelativeToNow: 5.Minutes()
                );

            return Command.Builder.WithName(cmdName)
                .WithState(this)
                .WithExecutePredicate(@this => @this.IsReadyToShow)
                .Asynchronously()
                .WithExecuteAction(async (@this, cancellationToken) =>
                {
                    await UniTask.DelayFrame(
                        delayFrameCount: 1,
                        delayTiming: PlayerLoopTiming.Update,
                        cancellationToken: cancellationToken
                        );

                    @this.ShowInternal();
                    @this.IsShown = true;
                    @this.OnShown();
                })
                .BuildPooled()
                .Value
                .WithCancellationToken(destroyCancellationToken);
        }

        private void OnUIReload(PanelRenderer _, VisualElement root)
        {
            rendererRoot = root;
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
