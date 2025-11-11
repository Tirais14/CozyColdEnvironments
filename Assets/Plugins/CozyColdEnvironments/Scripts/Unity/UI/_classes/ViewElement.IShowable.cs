using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI.Elements
{
    public partial class ViewElement : IShowable
    {
        protected readonly List<GraphicComponentStateSnapshot> showableGraphicSnapshots = new();

        [Space]
        [Header(nameof(Showable) + " Settings")]
        [Space]

        [SerializeField]
        protected IShowable.Settings showableSettings = IShowable.Settings.Default;

        [SerializeField]
        protected bool showOnStart;

        private readonly ReactiveProperty<bool> isVisible = new();

        public bool IsVisible => isVisible.Value;

        private void StartIShowable()
        {
            ShowablePreheat();

            if (showOnStart)
                Show();
        }

        public virtual bool ShowableShowAllowedPredicate() => true;

        public virtual void Hide(IShowable.Settings settings)
        {
            Hide(force: false);
        }
        public void Hide() => Hide(showableSettings);

        protected void Hide(bool force)
        {
            if (!force && !IsVisible)
                return;

            Showable.Hide(
                gameObject,
                showableGraphicSnapshots,
                showableSettings);

            isVisible.Value = false;
        }

        public virtual void Show(IShowable.Settings settings)
        {
            if (!ShowableShowAllowedPredicate())
                return;

            Showable.Show(
                gameObject,
                showableGraphicSnapshots,
                showableSettings);

            isVisible.Value = true;
        }
        public void Show() => Show(showableSettings);

        public bool SwitchVisibleState()
        {
            if (IsVisible)
                Hide();
            else
                Show();

            return IsVisible;
        }

        public IObservable<Unit> ObserveShow()
        {
            return isVisible.Where(_ => StartPassed).Where(x => x).AsUnitObservable();
        }

        public IObservable<Unit> ObserveHide()
        {
            return isVisible.Where(_ => StartPassed).Where(x => !x).AsUnitObservable();
        }

        private void ShowablePreheat()
        {
            Show();
            Hide(force: true);
        }
    }
}
