using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.UI.Elements;
using System;
using UniRx;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public class Showable : CCBehaviour, IShowable
    {
        public const string ALREADY_OPENED_MSG = "{0}/> Already opned.";
        public const string CANNOT_OPENED_LOG = "Cannot be opened by reasos: {0}.";

        protected readonly Subject<Unit> onOpenRx = new();
        protected readonly Subject<Unit> onCloseRx = new();
        protected readonly ReactiveProperty<bool> isVisible = new();

        public event Action? OnShow;
        public event Action? OnHide;

        public IObservable<Unit> OnShowRx => onOpenRx;
        public IObservable<Unit> OnHideRx => onCloseRx;
        public IReadOnlyReactiveProperty<bool> IsVisible => isVisible;

        protected virtual bool ShowOnStart => false;

        protected override void Awake()
        {
            base.Awake();

            gameObject.ObserveEveryValueChanged(x => x.activeSelf)
                      .Subscribe(x => isVisible.Value = x)
                      .AddTo(this);
        }

        protected override void Start()
        {
            base.Start();

            if (ShowOnStart)
                Show();
            else
                Hide();
        }

        public virtual bool CanShow(out string msg)
        {
            if (IsVisible.Value)
            {
                msg = ALREADY_OPENED_MSG.Format(GetType().GetFullName());
                return false;
            }

            msg = string.Empty;
            return true;
        }
        public bool CanShow() => CanShow(out _);

        public virtual void Hide()
        {
            onCloseRx.OnNext(Unit.Default);
            gameObject.SetActive(false);
        }

        public virtual void Show()
        {
            if (!CanShow(out string msg))
            {
                this.PrintLog(CANNOT_OPENED_LOG.Format(msg));
                return;
            }

            gameObject.SetActive(true);
            onOpenRx.OnNext(Unit.Default);
        }

        public bool SwitchVisibleState()
        {
            gameObject.SetActive(!IsVisible.Value);

            return gameObject.activeSelf;
        }
    }
}
