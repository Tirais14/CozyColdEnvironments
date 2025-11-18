using CCEnvs.FuncLanguage;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.UI
{
    [Serializable]
    public partial class GUIPanel : ISelectable
    {
        protected readonly ReactiveProperty<bool> isSelected = new();

        [Header("Selectable settings")]
        [Space(8)]

        [SerializeField]
        protected bool m_SelectableEnabled;

        [SerializeField]
        protected Maybe<Image> m_SelectionOverlayPrefab;

        public Image selectionOverlay { get; private set; } = null!;
        public bool SelectableEnabled {
            get => m_SelectableEnabled;
            set => m_SelectableEnabled = value;
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

            ISelectableSelectionOverlayTryInit();

            selectionOverlay.gameObject.SetActive(true);
            isSelected.Value = true;
        }

        public virtual void DoDeselect() => DoDeselect(force: false);

        protected void DoDeselect(bool force)
        {
            if (!force && !IsSelected)
                return;

            selectionOverlay.gameObject.SetActive(false);
            isSelected.Value = false;
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

        private void ISelectableOnTransformChildrenChanged()
        {
            if (selectionOverlay != null)
                selectionOverlay.transform.SetAsLastSibling();
        }

        private void ISelectableSelectionOverlayTryInit()
        {
            if (selectionOverlay != null)
                return;

            RectTransform overlayTransform;
            RectTransform thisTransfrom = transform.As<RectTransform>();

            m_SelectionOverlayPrefab.Match(
                some: prefab =>
                {
                    selectionOverlay = Instantiate(prefab, transform);
                    overlayTransform = selectionOverlay.transform.As<RectTransform>();

                    overlayTransform.position = thisTransfrom.position;
                    overlayTransform.sizeDelta = thisTransfrom.sizeDelta;
                },
                none: () =>
                {
                    var overlay = new GameObject(nameof(selectionOverlay), typeof(Image)).QueryTo()
                        .Component<Image>()
                        .Strict();

                    overlayTransform = overlay.transform.As<RectTransform>();

                    overlayTransform.SetParent(transform);
                    overlayTransform.position = transform.position;
                    overlay.gameObject.SetActive(isSelected.Value);
                    overlay.sprite = UCC.ColorSprite.Value;
                    overlay.color = Color.lightYellow.WithAlpha(0.45f);
                    overlayTransform.sizeDelta = thisTransfrom.sizeDelta;

                    selectionOverlay = overlay;
                });
        }
    }
}
