using CCEnvs.Unity.Components;
using System;
using UniRx;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public class Showable : CCBehaviour, IShowable, IDisposable
    {
        protected readonly ReactiveProperty<bool> isVisible = new();

        private Subject<Unit>? hideSubj;
        private Subject<Unit>? showSubj;
        private bool disposed;

        public event Action? OnShow;
        public event Action? OnHide;

        public IReadOnlyReactiveProperty<bool> IsVisible => isVisible;
        public bool IsShowable { get; private set; }

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

        public virtual void Hide()
        {
            OnHide?.Invoke();
            hideSubj?.OnNext(Unit.Default);
            gameObject.SetActive(false);
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
            showSubj?.OnNext(Unit.Default);
            OnShow?.Invoke();
        }

        public bool SwitchVisibleState()
        {
            gameObject.SetActive(!IsVisible.Value);

            return gameObject.activeSelf;
        }

        public IObservable<Unit> ObserveShow()
        {
            showSubj ??= new Subject<Unit>();
            return showSubj;
        }

        public IObservable<Unit> ObserveHide()
        {
            hideSubj ??= new Subject<Unit>();
            return hideSubj;
        }

        public void Dispose() => Dispose(disposing: true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            showSubj?.Dispose();
            hideSubj?.Dispose();

            disposed = true;
        }
    }
}
