using CCEnvs.Unity.Components;
using R3;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public class Selectable : CCBehaviour, ISelectable
    {
        protected readonly ReactiveProperty<bool> isSelected = new();
        protected readonly ReactiveProperty<bool> isEnabled = new();

        [SerializeField]
        protected Image m_SelectionOverlay;

        public Image SelectionOverlay => m_SelectionOverlay;

        public bool IsSelected => isSelected.Value;
        public bool IsEnabled => isEnabled.Value;

        protected override void Start()
        {
            base.Start();
            InitSelectionOverlay();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            isEnabled.Value = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            isEnabled.Value = false;
        }

        protected virtual void OnTransformChildrenChanged()
        {
            if (SelectionOverlay != null)
                SelectionOverlay.transform.SetAsLastSibling();
        }

        public virtual bool SelectAllowedPredicate() => enabled;

        public virtual void DoSelect()
        {
            if (!SelectAllowedPredicate())
                return;

            SelectionOverlay!.gameObject.SetActive(true);
            isSelected.Value = true;
        }

        public virtual void DoDeselect() => DoDeselect(force: false);

        protected void DoDeselect(bool force)
        {
            if (!force && !IsSelected)
                return;

            SelectionOverlay.gameObject.SetActive(false);
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
            SelectionOverlay.enabled = true;
        }

        public void Disable()
        {
            enabled = false;
            SelectionOverlay.enabled = false;
        }

        public Selectable SetSelectionOverlay(Image image)
        {
            CC.Guard.IsNotNull(image, nameof(image));

            image.gameObject.SetActive(isSelected.Value);
            image.transform.SetParent(transform);

            m_SelectionOverlay = image;

            return this;
        }

        public Observable<bool> ObserveIsSelected() => isSelected;

        public Observable<ISelectable> ObserveDoSelect()
        {
            return isSelected.Where(x => x).Select(_ => (ISelectable)this);
        }

        public Observable<ISelectable> ObserveDoDeselect()
        {
            return isSelected.Where(x => !x).Select(_ => (ISelectable)this);
        }

        public Observable<bool> ObserveEnabled()
        {
            return isEnabled.Where(static x => x);
        }

        public Observable<bool> ObserveDisabled()
        {
            return isEnabled.Where(static x => !x);
        }

        private void InitSelectionOverlay()
        {
            if (SelectionOverlay == null)
            {
                var overlay = new GameObject(nameof(SelectionOverlay), typeof(Image)).QueryTo()
                    .Component<Image>()
                    .Strict();

                overlay.gameObject.SetActive(isSelected.Value);
                overlay.color = Color.lightYellow.WithAlpha(0.45f);

                RectTransform overlayTransform = overlay.RectTransform();
                overlayTransform.position = transform.position;
                overlayTransform.sizeDelta = this.RectTransform().sizeDelta;

                SetSelectionOverlay(overlay);
            }
            else
                SelectionOverlay.gameObject.SetActive(isSelected.Value);
        }
    }
}
