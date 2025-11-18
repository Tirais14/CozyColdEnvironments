using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public partial class GUIPanel : IShowable
    {
        protected readonly List<GraphicComponentStateSnapshot> showableGraphicSnapshots = new();

        [Header("Showable settings")]
        [Space(8)]

        [SerializeField]
        protected IShowable.Settings m_ShowableSettings = IShowable.Settings.Default;

        [SerializeField]
        protected bool m_ShowOnStart;

        public IShowable.Settings ShowableSettings {
            get => m_ShowableSettings;
            set => m_ShowableSettings = value;
        }
        public bool ShowOnStart {
            get => m_ShowOnStart;
            set => m_ShowOnStart = value;
        }

        private readonly ReactiveProperty<bool> isVisible = new();

        public bool IsVisible => isVisible.Value;

        private void IShowableStart()
        {
            ShowablePreheat();

            if (ShowOnStart)
                Show();
        }

        private void IShowableOnTransformChildrenChanged()
        {
            PreUpdateAction(Redraw);
        }

        public virtual bool ShowableShowAllowedPredicate() => true;

        public virtual void Hide(IShowable.Settings settings)
        {
            Hide(force: false, settings);
        }
        public void Hide() => Hide(ShowableSettings);

        protected void Hide(bool force, IShowable.Settings settings)
        {
            if (!force && !IsVisible)
                return;

            Showable.Hide(gameObject,
                showableGraphicSnapshots,
                settings);

            isVisible.Value = false;
        }

        public virtual void Show(IShowable.Settings settings)
        {
            if (!ShowableShowAllowedPredicate())
                return;

            Showable.Show(gameObject,
                showableGraphicSnapshots,
                ShowableSettings);

            isVisible.Value = true;
        }
        public void Show() => Show(ShowableSettings);

        public bool SwitchVisibleState()
        {
            if (IsVisible)
                Hide();
            else
                Show();

            return IsVisible;
        }

        public void Redraw()
        {
            if (IsVisible)
            {
                Hide();
                Show();
            }
            else
            {
                Show();
                Hide();
            }
        }

        public IObservable<Unit> ObserveShow()
        {
            return isVisible.Where(_ => StartPassed).Where(x => x).AsUnitObservable();
        }

        public IObservable<Unit> ObserveHide()
        {
            return isVisible.Where(_ => StartPassed).Where(x => !x).AsUnitObservable();
        }

        //To reset visibility state
        private void ShowablePreheat()
        {
            Hide(force: true, ShowableSettings);
        }
    }
}
