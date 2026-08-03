using CCEnvs.Snapshots;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public record PointerEventSnapshot : Snapshot<IPointerEvent>
    {
        public int pointerId { get; set; }
        public string pointerType { get; set; } = string.Empty;
        public bool isPrimary { get; set; }
        public int button { get; set; }
        public int pressedButtons { get; set; }
        public Vector3 position { get; set; }
        public Vector3 localPosition { get; set; }
        public Vector3 deltaPosition { get; set; }
        public float deltaTime { get; set; }
        public int clickCount { get; set; }
        public float pressure { get; set; }
        public float tangentialPressure { get; set; }
        public float altitudeAngle { get; set; }
        public float azimuthAngle { get; set; }
        public float twist { get; set; }
        public Vector2 tilt { get; set; }
        public PenStatus penStatus { get; set; }
        public Vector2 radius { get; set; }
        public Vector2 radiusVariance { get; set; }
        public EventModifiers modifiers { get; set; }
        public bool shiftKey { get; set; }
        public bool ctrlKey { get; set; }
        public bool commandKey { get; set; }
        public bool altKey { get; set; }
        public bool actionKey { get; set; }

        public PointerEventSnapshot()
        {
        }

        public PointerEventSnapshot(Snapshot<IPointerEvent> original) : base(original)
        {
        }

        public PointerEventSnapshot(IPointerEvent target) : base(target)
        {
        }

        protected override void OnRestore(ref IPointerEvent target) { }

        public override bool CanRestore(IPointerEvent? target) => false;

        protected override void OnCapture(IPointerEvent target)
        {
            base.OnCapture(target);
            pointerId = target.pointerId;
            pointerType = target.pointerType;
            isPrimary = target.isPrimary;
            button = target.button;
            pressedButtons = target.pressedButtons;
            position = target.position;
            localPosition = target.localPosition;
            deltaPosition = target.deltaPosition;
            deltaTime = target.deltaTime;
            clickCount = target.clickCount;
            pressure = target.pressure;
            tangentialPressure = target.tangentialPressure;
            altitudeAngle = target.altitudeAngle;
            azimuthAngle = target.azimuthAngle;
            twist = target.twist;
            tilt = target.tilt;
            penStatus = target.penStatus;
            radius = target.radius;
            radiusVariance = target.radiusVariance;
            modifiers = target.modifiers;
            shiftKey = target.shiftKey;
            ctrlKey = target.ctrlKey;
            commandKey = target.commandKey;
            altKey = target.altKey;
        }

        protected override void OnReset()
        {
            base.OnReset();
            pointerId = default;
            pointerType = default!;
            isPrimary = default;
            button = default;
            pressedButtons = default;
            position = default;
            localPosition = default;
            deltaPosition = default;
            deltaTime = default;
            clickCount = default;
            pressure = default;
            tangentialPressure = default;
            altitudeAngle = default;
            azimuthAngle = default;
            twist = default;
            tilt = default;
            penStatus = default;
            radius = default;
            radiusVariance = default;
            modifiers = default;
            shiftKey = default;
            ctrlKey = default;
            commandKey = default;
            altKey = default;
        }
    }
}
