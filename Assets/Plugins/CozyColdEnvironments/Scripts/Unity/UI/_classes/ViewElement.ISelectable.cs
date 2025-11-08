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
        protected bool selectableDisabled;

        [SerializeField]
        protected Color selectableSelectionColor = Color.red;

        protected Color selectableBeforeSelectColor;

        public bool IsSelected => isSelected.Value; 

        private void StartISelectable()
        {
            SelectablePreheat();
            SelectableBindButton();
        }

        public virtual bool SelectAllowedPredicate(out Maybe<string> msg)
        {
            msg = null;
            return true;
        }

        public virtual void DoSelect()
        {
            if (selectableDisabled || IsSelected)
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

        public IObservable<Unit> ObserveSelect()
        {
            return isSelected.Where(x => x).AsUnitObservable();
        }

        public IObservable<Unit> ObserveDeselect()
        {
            return isSelected.Where(x => !x).AsUnitObservable();
        }

        private void SelectableBindButton()
        {
            button.IfSome(button => button.OnClickAsObservable()
                .SubscribeWithState(this, (_, @this) => @this.SwitchSelectionState())
                .AddTo(this)
                );
        }

        private void SelectablePreheat()
        {
            DoSelect();
            DoDeselect();
        }
    }
}
