using CCEnvs.Unity.Components;
using R3;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.UI.Components
{
    public class RectTransformSizeBinding : CCBehaviour, IFrameRunnerWorkItem
    {
        [SerializeField]
        protected RectTransform _target;

        [SerializeField]
        protected bool observeWidth = true;

        [SerializeField]
        protected bool observeHeight = true;

        [SerializeField]
        protected float widthMargin;

        [SerializeField]
        protected float heightMargin;

        private RectTransform? _transform;

        private Vector2 targetSizeDeltaSnapshot;

        public RectTransform Target {
            get => _target;
            set => _target = value;
        }

        public bool ObserveWidth {
            get => observeWidth;
            set => observeWidth = value;
        }

        public bool ObserveHeight {
            get => observeHeight;
            set => observeHeight = value;
        }

        public float WidthMargin {
            get => widthMargin;
            set => widthMargin = value;
        }

        public float HeightMargin {
            get => heightMargin;
            set => heightMargin = value;
        }

        new public RectTransform transform {
            get
            {
                if (_transform == null)
                    _transform = this.RectTransform();

                return _transform;
            }
        }

        protected override void Start()
        {
            base.Start();

            UnityFrameProvider.PostLateUpdate.Register(this);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            if (_target == null)
                return;

            OnTargetSizeDeltaChanged();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            OnEnable();
        }
#endif

        private void OnTargetSizeDeltaChanged()
        {
            var targetSizeDelta = _target.sizeDelta;

            if (observeWidth)
                targetSizeDelta = targetSizeDelta.AddX(widthMargin);

            if (observeHeight)
                targetSizeDelta = targetSizeDelta.AddY(heightMargin);

            transform.sizeDelta = targetSizeDelta;
        }

        bool IFrameRunnerWorkItem.MoveNext(long frameCount)
        {
            if (IsDestroyed)
                return false;

            if (!isActiveAndEnabled || frameCount % 3 != 0 || _target == null)
                return true;

            if (targetSizeDeltaSnapshot != Target.sizeDelta)
                OnTargetSizeDeltaChanged();

            targetSizeDeltaSnapshot = Target.sizeDelta;

            return true;
        }
    }
}
