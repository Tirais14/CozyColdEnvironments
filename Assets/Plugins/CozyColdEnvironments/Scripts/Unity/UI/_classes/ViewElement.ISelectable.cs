using CCEnvs.FuncLanguage;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.UI.Elements
{
    [Serializable]
    public partial class ViewElement : ISelectable
    {
        protected readonly ReactiveProperty<bool> isSelected = new();

        [Header("Selectable settings")]
        [Space(8)]

        [SerializeField]
        protected bool m_SelectableEnabled;

        [SerializeField]
        protected Maybe<Image> m_SelectionOverlay;

        [SerializeField]
        protected Color m_SelectableSelectionColor = Color.lightYellow.WithAlpha(0.25f);

        public bool SelectableEnabled {
            get => m_SelectableEnabled;
            set => m_SelectableEnabled = value;
        }
        public Maybe<Image> selectionOverlay {
            get => m_SelectionOverlay;
            set => m_SelectionOverlay = value;
        }
        public Color selectableSelectionColor {
            get => m_SelectableSelectionColor;
            set => m_SelectableSelectionColor = value;
        }
        public bool IsSelected => isSelected.Value;

        bool ISelectable.IsEnabled {
            get => SelectableEnabled;
            set => SelectableEnabled = value;    
        }

        public virtual bool SelectAllowedPredicate() => SelectableEnabled && !IsSelected;

        public virtual void DoSelect()
        {
            if (!SelectAllowedPredicate())
                return;

            SelectableSelectionOverlayTryInit();

            isSelected.Value = true;
            selectionOverlay.GetValueUnsafe().gameObject.SetActive(IsSelected);
        }

        public virtual void DoDeselect() => DoDeselect(force: false);

        protected void DoDeselect(bool force)
        {
            if (selectionOverlay.IsNone || (!force && !IsSelected))
                return;

            isSelected.Value = false;
            selectionOverlay.GetValueUnsafe().gameObject.SetActive(IsSelected);
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

        private void SelectableOnTransformChildrenChanged()
        {
            selectionOverlay.IfSome(img => img.transform.SetAsLastSibling());
        }

        private void SelectableSelectionOverlayTryInit()
        {
            if (selectionOverlay.IsNone)
            {
                var t = new GameObject(nameof(selectionOverlay), typeof(Image)).QueryTo()
                    .Component<Image>()
                    .Strict();

                t.transform.SetParent(transform);
                t.gameObject.SetActive(isSelected.Value);

                selectionOverlay = t;
            }
        }
    }
}
