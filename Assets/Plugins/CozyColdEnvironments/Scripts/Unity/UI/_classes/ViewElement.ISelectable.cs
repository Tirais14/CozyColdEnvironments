using CCEnvs.FuncLanguage;
using System;
using UniRx;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI.Elements
{
    public partial class ViewElement : ISelectable
    {
        protected readonly ReactiveProperty<bool> isSelected = new();

        [SerializeField]
        protected Color selectableSelectionColor = Color.red;

        protected Color selectableBeforeSelectColor;
        private Subject<Unit>? selectableSelectSubj;
        private Subject<Unit>? selectableDeselectSubj;

        public IReadOnlyReactiveProperty<bool> IsSelected => isSelected;

        private void StartISelectable()
        {
            SelectablePreheat();
        }

        public virtual bool SelectPredicate(out Maybe<string> msg)
        {
            msg = null;
            return true;
        }

        public virtual void DoSelect()
        {
            if (isSelected.Value)
                return;

            isSelected.Value = true;

            Img.IfSome(img =>
            {
                selectableBeforeSelectColor = img.color;
                img.color *= selectableSelectionColor;
            });
        }

        public virtual void DoDeselect()
        {
            if (!isSelected.Value)
                return;

            isSelected.Value = false;

            Img.IfSome(img => img.color = selectableBeforeSelectColor);
        }

        public void SwitchSelectionState()
        {
            if (isSelected.Value)
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
