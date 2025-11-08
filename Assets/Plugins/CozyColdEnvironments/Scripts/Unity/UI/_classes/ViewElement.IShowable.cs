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

        [SerializeField]
        protected IShowable.Settings showableSettings = IShowable.Settings.Default;

        [SerializeField]
        protected bool showOnStart;

        private readonly ReactiveProperty<bool> isVisible = new();

        public bool IsVisible => isVisible.Value;

        private void StartIShowable()
        {
            if (showOnStart)
                Show();
            else
                Hide();
        }

        public virtual void Hide(IShowable.Settings settings)
        {
            Showable.Hide(
                gameObject,
                showableGraphicSnapshots,
                showableSettings);

            isVisible.Value = false;
        }
        public void Hide() => Hide(showableSettings);

        public virtual void Show(IShowable.Settings settings)
        {
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
            return isVisible.Where(x => x).AsUnitObservable();
        }

        public IObservable<Unit> ObserveHide()
        {
            return isVisible.Where(x => !x).AsUnitObservable();
        }
    }
}
