using CCEnvs.Diagnostics;
using CCEnvs.Unity.Components;
using System;
using UniRx;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public class Showable : CCBehaviour, IShowable
    {
        protected readonly ReactiveProperty<bool> isVisible = new();

        private Subject<Unit>? hideSubj;
        private Subject<Unit>? showSubj;

        public event Action? OnShow;
        public event Action? OnHide;

        public IReadOnlyReactiveProperty<bool> IsVisible => isVisible;
        public virtual bool IsShowable => true;

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

        protected virtual void OnDestroy()
        {
            showSubj?.Dispose();
            hideSubj?.Dispose();
        }

        public virtual void Hide()
        {
            try
            {
                OnHide?.Invoke();
                hideSubj?.OnNext(Unit.Default);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }

            foreach (var cmp in GetComponents<Graphic>())
                cmp.enabled = false;
        }

        public virtual void Show()
        {
            foreach (var cmp in GetComponents<Graphic>())
                cmp.enabled = true;

            try
            {
                OnShow?.Invoke();
                showSubj?.OnNext(Unit.Default);
            }
            catch (Exception ex)
            {
                this.PrintException(ex);
            }
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
    }
}
