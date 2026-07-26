using CCEnvs.Collections;
using CCEnvs.FuncLanguage;
using CCEnvs.Patterns.Commands;
using CCEnvs.Threading;
using CCEnvs.TypeMatching;
using CCEnvs.UnityX.Components;
using CCEnvs.UnityX.UI.Elements;
using Cysharp.Threading.Tasks;
using Humanizer;
using R3;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.UI
{
    public abstract class ShowableBase<TSelf> : CCBehaviour, IShowableBase
        where TSelf : IShowableBase
    {
        [Header("Showable settings")]
        [Space(8)]

        [SerializeField]
        protected bool showOnInited;

        [SerializeField]
        protected bool preventHide;

        [SerializeField]
        protected bool isEnabled = true;

        protected readonly CommandScheduler commandScheduler = new(UnityFrameProvider.Update, nameof(Showable));

        protected bool isLayoutsRebuilding;
        protected bool isInitFaulted;

        private readonly ReactiveProperty<bool> isShown = new(true);

        private MonoBehaviour? _root;
        private MonoBehaviour? _parent;

        private ReactiveCommand<bool>? onInitedEvent;

        public bool ShowOnInited {
            get => showOnInited;
            set => showOnInited = value;
        }

        public virtual bool IsShown {
            get => isShown.Value;
            protected set => isShown.Value = value;
        }
        public bool IsInited { get; protected set; }
        public virtual bool IsReadyToShow => didStart && IsEnabled;

        public bool IsEnabled {
            get => isEnabled;
            set => isEnabled = value;
        }

        public bool PreventHide {
            get => preventHide;
            set => preventHide = value;
        }

        public TSelf? ShowableRoot => _root.As<TSelf>();
        public TSelf? Parent => _parent.As<TSelf>();

        protected override void Awake()
        {
            base.Awake();
            commandScheduler.Disable(); //disabling until IsInited
            ObserveTransformParent();
        }

        protected override void Start()
        {
            base.Start();
            SetRoot();
            SetParent();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            commandScheduler.Dispose();
            isShown.Dispose();
        }

        public async UniTask WaitUntilInited(CancellationToken cancellationToken = default)
        {
            if (IsInited)
                return;

            ThrowIfInitFailured();

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
            ThrowIfInitFailured();

            if (!IsEnabled || PreventHide)
                return;

            GetHideCommand(destroyCancellationToken).ScheduleBy(commandScheduler);
        }

        public async UniTask HideAsync(CancellationToken cancellationToken = default)
        {
            if (!IsShown || PreventHide)
                return;

            ThrowIfInitFailured();

            if (!IsEnabled)
                return;

            using var _ = destroyCancellationToken.TryLinkTokens(
                cancellationToken,
                out cancellationToken
                );

            cancellationToken.ThrowIfCancellationRequested();

            await GetHideCommand(cancellationToken).ScheduleBy(commandScheduler)
                .ObserveIsDone()
                .FirstAsync(cancellationToken);
        }

        public void Show()
        {
            ThrowIfInitFailured();

            if (!IsEnabled)
                return;

            GetShowCommand(destroyCancellationToken).ScheduleBy(commandScheduler);
        }

        public async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            if (IsShown)
                return;

            ThrowIfInitFailured();

            if (!IsEnabled)
                return;

            using var _ = destroyCancellationToken.TryLinkTokens(
                cancellationToken,
                out cancellationToken
                );

            cancellationToken.ThrowIfCancellationRequested();

            await GetShowCommand(cancellationToken).ScheduleBy(commandScheduler)
                .ObserveIsDone()
                .FirstAsync(cancellationToken);
        }

        public bool SwitchShownState()
        {
            if (!IsEnabled)
                return IsShown;

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
            if (!IsEnabled)
                return IsShown;

            if (IsShown)
            {
                await HideAsync();
                return false;
            }
            else
            {
                await ShowAsync();
                return true;
            }
        }

        public void SwitchShownStateVoid() => SwitchShownState();

        public abstract void Redraw();

        public async UniTask RedrawAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfInitFailured();

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
            if (IsInited)
                return Observable.Return(true);

            onInitedEvent ??= new ReactiveCommand<bool>();

            return onInitedEvent.Prepend(IsInited);
        }

        public TSelf[] GetDirectChilds()
        {
            return this.Q()
                .FromChildrens()
                .ExcludeSelf()
                .FirstComponentsOnBranch()
                .Components<TSelf>()
                .ToArray();
        }

        public T[] GetChilds<T>()
        {
            return this.Q()
                .FromChildrens()
                .WithDepthLimiter<IShowable>()
                .Components<T>()
                .ToArray();
        }

        protected virtual void OnHide()
        {
        }

        protected abstract void HideCore();

        protected virtual void OnHiden()
        {
        }

        protected virtual void OnShow()
        {
        }

        protected abstract void ShowCore();

        protected virtual void OnShown()
        {
        }

        protected virtual void ExecuteOnInitedEvent()
        {
            onInitedEvent?.Execute(IsInited);
        }

        protected void ThrowIfInitFailured()
        {
            if (isInitFaulted)
                throw new InvalidOperationException($"{nameof(GUITab)}: {this} is not correctly initialized");
        }


        protected void ShowInternal()
        {
            OnShow();
            ShowCore();
        }

        protected void HideInternal()
        {
            OnHide();
            HideCore();
        }

        protected async UniTask InitVisibleStateAsync()
        {
            destroyCancellationToken.ThrowIfCancellationRequested();

            if (ShowOnInited)
                ShowInternal();
            else
                HideInternal();
        }

        protected async UniTask WaitUntilChildrensInitedAsync()
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
                        foreach (var child in childs)
                            if (!child.IsInited)
                                return false;

                        return true;
                    },
                    cancellationToken: destroyCancellationToken
                    );
            }
        }

        protected virtual ICommandBase GetHideCommand(CancellationToken cancellationToken)
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
                static @this =>
                {
                    @this.HideInternal();
                    @this.IsShown = false;
                    @this.OnHiden();
                })
                .BuildPooled()
                .Value
                .WithCancellationToken(cancellationToken);
        }

        protected virtual ICommandBase GetShowCommand(CancellationToken cancellationToken)
        {
            string cmdName = NameFactory.CreateFromCaller(
                this,
                nameof(Show),
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
                static @this =>
                {
                    @this.ShowInternal();
                    @this.IsShown = true;
                    @this.OnShown();
                })
                .BuildPooled()
                .Value
                .WithCancellationToken(cancellationToken);
        }

        private void SetRoot()
        {
            foreach (var showable in transform.root.Q()
                .FromChildrens()
                .IncludeInactive()
                .FirstComponentsOnBranch()
                .Components<IShowableBase>())
            {
                if (showable.IsNot<MonoBehaviour>(out var monoShowable))
                    continue;

                if (monoShowable == this)
                    break;

                if (transform.IsChildOf(monoShowable.transform))
                {
                    _root = monoShowable;
                    break;  
                }
            }
        }

        private void SetParent()
        {
            _parent = this.Q()
                .FromParents()
                .ExcludeSelf()
                .IncludeInactive()
                .Component<IShowableBase>()
                .Lax()
                .Cast<MonoBehaviour>()
                .GetValue();
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
