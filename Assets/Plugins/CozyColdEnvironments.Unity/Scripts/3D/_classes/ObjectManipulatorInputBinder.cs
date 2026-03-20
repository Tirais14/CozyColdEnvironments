using CCEnvs.Dependencies;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using CCEnvs.Unity.InputSystem.Rx;
using R3;
using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.D3
{
    [DisallowMultipleComponent]
    public sealed class ObjectManipulatorInputBinder : CCBehaviour
    {
        [Header("Translation Input Actions")]
        [Space(6f)]

        [SerializeField]
        private InputActionRxReference<Vector2> distanceOffsetInputAction;

        [SerializeField]
        private InputActionRxReference<float> horizontalOffsetInputAction;

        [SerializeField]
        private InputActionRxReference<float> verticalOffsetInputAction;

        [Header("Rotation Input Actions")]
        [Space(6f)]

        [SerializeField]
        private InputActionRxReference<float> pitchOffsetInputAction;

        [SerializeField]
        private InputActionRxReference<float> yawOffsetInputAction;

        [SerializeField]
        private InputActionRxReference<float> rollOffsetInputAction;

        [GetBySelf]
        private ObjectManipulator objManipulator = null!;

        private IDisposable? distanceOffsetIABinding;
        private IDisposable? horizontalOffsetIABinding;
        private IDisposable? verticalOffsetIABinding;
        private IDisposable? pitchOffsetIABinding;
        private IDisposable? yawOffsetIABinding;
        private IDisposable? rollOffsetIABinding;

        public InputActionRx<Vector2>? DistanceOffsetIA { get; private set; }
        public InputActionRx<float>? HorizontalOffsetIA { get; private set; }
        public InputActionRx<float>? VerticalOffsetIA { get; private set; }
        public InputActionRx<float>? PitchOffsetIA { get; private set; }
        public InputActionRx<float>? YawOffsetIA { get; private set; }
        public InputActionRx<float>? RollOffsetIA { get; private set; }

        protected override void Start()
        {
            base.Start();

            if (DistanceOffsetIA == null && distanceOffsetInputAction != null && distanceOffsetInputAction.IsValid)
            {
                DistanceOffsetIA = distanceOffsetInputAction.Value;
                TryBindDistanceOffsetIA();
            }

            if (HorizontalOffsetIA == null && horizontalOffsetInputAction != null && horizontalOffsetInputAction.IsValid)
            {
                HorizontalOffsetIA = horizontalOffsetInputAction.Value;
                TryBindHorizontalOffsetIA();
            }

            if (VerticalOffsetIA == null && verticalOffsetInputAction != null && verticalOffsetInputAction.IsValid)
            {
                VerticalOffsetIA = verticalOffsetInputAction.Value;
                TryBindVerticalOffsetIA();
            }

            if (PitchOffsetIA == null && pitchOffsetInputAction != null && pitchOffsetInputAction.IsValid)
            {
                PitchOffsetIA = pitchOffsetInputAction.Value;
                TryBindPitchOffsetIA();
            }

            if (YawOffsetIA == null && yawOffsetInputAction != null && yawOffsetInputAction.IsValid)
            {
                YawOffsetIA = yawOffsetInputAction.Value;
                TryBindYawOffsetIA();
            }

            if (RollOffsetIA == null && rollOffsetInputAction != null && rollOffsetInputAction.IsValid)
            {
                RollOffsetIA = rollOffsetInputAction.Value;
                TryBindRollOffsetIA();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            distanceOffsetIABinding?.Dispose();
            horizontalOffsetIABinding?.Dispose();
            verticalOffsetIABinding?.Dispose();
            pitchOffsetIABinding?.Dispose();
            yawOffsetIABinding?.Dispose();
            rollOffsetIABinding?.Dispose();
        }

        public ObjectManipulatorInputBinder SetDistanceOffsetIA(InputActionRx<Vector2>? ia)
        {
            DistanceOffsetIA = ia;
            TryBindDistanceOffsetIA();
            return this;
        }

        public ObjectManipulatorInputBinder SetHorizontalOffsetIA(InputActionRx<float>? ia)
        {
            HorizontalOffsetIA = ia;
            TryBindHorizontalOffsetIA();
            return this;
        }

        public ObjectManipulatorInputBinder SetVerticalOffsetIA(InputActionRx<float>? ia)
        {
            VerticalOffsetIA = ia;
            TryBindVerticalOffsetIA();
            return this;
        }

        public ObjectManipulatorInputBinder SetPitchOffsetIA(InputActionRx<float>? ia)
        {
            PitchOffsetIA = ia;
            TryBindPitchOffsetIA();
            return this;
        }

        public ObjectManipulatorInputBinder SetYawOffsetIA(InputActionRx<float>? ia)
        {
            YawOffsetIA = ia;
            TryBindYawOffsetIA();
            return this;
        }

        public ObjectManipulatorInputBinder SetRollOffsetIA(InputActionRx<float>? ia)
        {
            RollOffsetIA = ia;
            TryBindRollOffsetIA();
            return this;
        }

        private void TryBindDistanceOffsetIA()
        {
            distanceOffsetIABinding?.Dispose();
            distanceOffsetIABinding = null;

            if (DistanceOffsetIA == null)
                return;

            distanceOffsetIABinding = DistanceOffsetIA.ObservePerformedValue()
                .Subscribe(OnDistanceOffsetIA);
        }

        private void TryBindHorizontalOffsetIA()
        {
            horizontalOffsetIABinding?.Dispose();
            horizontalOffsetIABinding = null;

            if (HorizontalOffsetIA == null)
                return;

            horizontalOffsetIABinding = HorizontalOffsetIA.ObservePerformedValue()
                .Subscribe(OnHorizontalOffsetIA);
        }

        private void TryBindVerticalOffsetIA()
        {
            verticalOffsetIABinding?.Dispose();
            verticalOffsetIABinding = null;

            if (VerticalOffsetIA == null)
                return;

            verticalOffsetIABinding = VerticalOffsetIA.ObservePerformedValue()
                .Subscribe(OnVerticalOffsetIA);
        }

        private void TryBindPitchOffsetIA()
        {
            pitchOffsetIABinding?.Dispose();
            pitchOffsetIABinding = null;

            if (PitchOffsetIA == null)
                return;

            pitchOffsetIABinding = PitchOffsetIA.ObservePerformedValue()
                .Subscribe(OnPitchOffsetIA);
        }

        private void TryBindYawOffsetIA()
        {
            yawOffsetIABinding?.Dispose();
            yawOffsetIABinding = null;

            if (YawOffsetIA == null)
                return;

            yawOffsetIABinding = YawOffsetIA.ObservePerformedValue()
                .Subscribe(OnYawOffsetIA);
        }

        private void TryBindRollOffsetIA()
        {
            rollOffsetIABinding?.Dispose();
            rollOffsetIABinding = null;

            if (RollOffsetIA == null)
                return;

            rollOffsetIABinding = RollOffsetIA.ObservePerformedValue()
                .Subscribe(OnRollOffsetIA);
        }

        private void OnDistanceOffsetIA(Vector2 inputValue)
        {
            if (inputValue.NearlyEquals(Vector2.zero))
                return;

            objManipulator.ApplyTranslation(new Vector3(0f, 0f, inputValue.y));
        }

        private void OnHorizontalOffsetIA(float inputValue)
        {
            if (inputValue.NearlyEquals(0f))
                return;

            objManipulator.ApplyTranslation(new Vector3(inputValue, 0f, 0f));
        }

        private void OnVerticalOffsetIA(float inputValue)
        {
            if (inputValue.NearlyEquals(0f))
                return;

            objManipulator.ApplyTranslation(new Vector3(0f, inputValue, 0f));
        }

        private void OnPitchOffsetIA(float inputValue)
        {
            if (inputValue.NearlyEquals(0f))
                return;

            objManipulator.ApplyRotation(new Vector3(inputValue, 0f, 0f));
        }

        private void OnYawOffsetIA(float inputValue)
        {
            if (inputValue.NearlyEquals(0f))
                return;

            objManipulator.ApplyRotation(new Vector3(0f, inputValue, 0f));
        }

        private void OnRollOffsetIA(float inputValue)
        {
            if (inputValue.NearlyEquals(0f))
                return;

            objManipulator.ApplyRotation(new Vector3(0f, 0f, inputValue));
        }
    }
}
