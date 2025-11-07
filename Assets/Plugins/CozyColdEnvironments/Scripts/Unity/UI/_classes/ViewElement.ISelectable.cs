using CCEnvs.FuncLanguage;
using System;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI.Elements
{
    public partial class ViewElement : ISelectable
    {
        [SerializeField]
        protected Color selectableSelectionColor = Color.red;

        protected Color selectableBeforeSelectColor;
        private Subject<Unit>? selectableSelectSubj;
        private Subject<Unit>? selectableDeselectSubj;

        public bool IsSelected { get; protected set; }

        private void StartISelectable()
        {
            SelectablePreheat();
        }

        public virtual bool SelectAllowedPredicate(out Maybe<string> msg)
        {
            msg = null;
            return true;
        }

        public virtual void DoSelect()
        {
            if (IsSelected)
                return;

            IsSelected = true;

            Img.IfSome(img =>
            {
                selectableBeforeSelectColor = img.color;
                img.color *= selectableSelectionColor;
            });
        }

        public virtual void DoDeselect()
        {
            if (!IsSelected)
                return;

            IsSelected = false;

            Img.IfSome(img => img.color = selectableBeforeSelectColor);
        }

        public void SwitchSelectionState()
        {
            if (IsSelected)
                DoDeselect();
            else
                DoSelect();
        }

        public IObservable<Unit> ObserveSelect()
        {
            selectableSelectSubj ??= new Subject<Unit>();

            return selectableSelectSubj;
        }

        public IObservable<Unit> ObserveDeselect()
        {
            selectableDeselectSubj ??= new Subject<Unit>();

            return selectableDeselectSubj;
        }

        private void SelectablePreheat()
        {
            DoSelect();
            DoDeselect();
        }
    }
}
