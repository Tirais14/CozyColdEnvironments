using CCEnvs.FuncLanguage;
using CCEnvs.Unity.Components;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public class Selectable : CCBehaviour, ISelectable
    {
        protected readonly ReactiveProperty<bool> isSelected = new();

        [SerializeField]
        protected Maybe<Image> m_SelectionOverlayPrefab;

        public Image selectionOverlay { get; private set; } = null!;
        public bool IsSelected => isSelected.Value;
        public bool IsEnabled => enabled;

        protected override void Start()
        {
            base.Start();
        }

        private void OnTransformChildrenChanged()
        {
            if (selectionOverlay != null)
                selectionOverlay.transform.SetAsLastSibling();
        }

        public virtual bool SelectAllowedPredicate() => enabled;

        public virtual void DoSelect()
        {
            if (!SelectAllowedPredicate())
                return;

            if (selectionOverlay == null)
                CreateSelectionOverlay();

            selectionOverlay!.gameObject.SetActive(true);
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

        public void Enable()
        {
            enabled = true;
            selectionOverlay.enabled = true;
        }

        public void Disable()
        {
            enabled = false;
            selectionOverlay.enabled = false;
        }

        public IObservable<bool> ObserveIsSelected()
        {
            return isSelected.Where(x => x);
        }

        public IObservable<Unit> ObserveDoSelect()
        {
            return isSelected.Where(x => x).AsUnitObservable();
        }

        public IObservable<Unit> ObserveDoDeselect()
        {
            return isSelected.Where(x => !x).AsUnitObservable();
        }

        private void CreateSelectionOverlay()
        {
  
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
