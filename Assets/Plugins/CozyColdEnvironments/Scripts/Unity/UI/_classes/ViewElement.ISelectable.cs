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

        [Space]
        [Header(nameof(Selectable) + " Settings")]
        [Space]

        [SerializeField]
        protected bool selectableDisabled;

        [SerializeField]
        protected Color selectableSelectionColor = Color.red;

        protected Color selectableBeforeSelectColor;

        public bool IsSelected => isSelected.Value; 

        private void StartISelectable()
        {
            SelectablePreheat();
        }

        public virtual bool SelectAllowedPredicate()
        {
            return !selectableDisabled;
        }

        public virtual void DoSelect()
        {
            if (!SelectAllowedPredicate() || IsSelected)
                return;

            isSelected.Value = true;

            image.IfSome(img =>
            {
                selectableBeforeSelectColor = img.color;
                img.color *= selectableSelectionColor;
            });
        }

        public virtual void DoDeselect()
        {
            if (!IsSelected)
                return;

            isSelected.Value = false;

            image.IfSome(img => img.color = selectableBeforeSelectColor);
        }

        public void SwitchSelectionState()
        {
            if (IsSelected)
                DoDeselect();
            else
                DoSelect();
        }

        public IObservable<bool> ObserveIsSelected()
        {
            return isSelected.Where(x => x);
        }

        private void SelectablePreheat()
        {
            DoSelect();
            DoDeselect();
        }
    }
}
