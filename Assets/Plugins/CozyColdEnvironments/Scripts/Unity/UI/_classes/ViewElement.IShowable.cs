using CCEnvs.FuncLanguage;
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
        private Subject<Unit>? showableShowSubj;
        private Subject<Unit>? showableHideSubj;

        public bool IsVisible { get; protected set; }
        public virtual bool IsShowAllowed => parentIsVisible;
        protected virtual bool showOnStart { get; }

        private void StartIShowable()
        {
            ShowablePreheat();

            if (showOnStart)
                Show();
            else
                Hide(showableSettings);
        }

        public virtual void Hide(IShowable.Settings settings)
        {
            if (!IsVisible)
                return;

            Showable.Hide(
                gameObject,
                showableGraphicSnapshots!,
                showableSettings);

            IsVisible = false;
        }
        public void Hide() => Hide(showableSettings);

        public virtual void Show(IShowable.Settings settings)
        {
            if (IsVisible)
                return;
            if (!IsShowAllowed)
                return;

            Showable.Show(
                gameObject,
                showableGraphicSnapshots,
                showableSettings);

            IsVisible = true;
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
            showableShowSubj ??= new Subject<Unit>();

            return showableShowSubj;
        }

        public IObservable<Unit> ObserveHide()
        {
            showableHideSubj ??= new Subject<Unit>();

            return showableHideSubj;
        }

        private void ShowablePreheat()
        {
            Show();
            Hide();
        }
    }
}
