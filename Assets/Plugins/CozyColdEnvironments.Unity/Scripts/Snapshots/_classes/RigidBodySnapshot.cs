using System;
using CCEnvs.Attributes.Serialization;
using Newtonsoft.Json;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public record RigidBodySnapshot<T> : ComponentSnapshot<T>
        where T : Rigidbody
    {
        [JsonIgnore]
        [SerializeField]
        protected Vector3? linearVelocity;

        [JsonIgnore]
        [SerializeField]
        protected Vector3? angularVelocity;

        [JsonIgnore]
        [SerializeField]
        protected float? linearDamping;

        [JsonIgnore]
        [SerializeField]
        protected float? angularDamping;

        [JsonIgnore]
        [SerializeField]
        protected float? mass;

        [JsonIgnore]
        [SerializeField]
        protected bool? useGravity;

        [JsonIgnore]
        [SerializeField]
        protected bool? isKinematic;

        [JsonIgnore]
        [SerializeField]
        protected RigidbodyInterpolation? interpolation;

        [JsonIgnore]
        [SerializeField]
        protected CollisionDetectionMode? collisionDetectionMode;

        [JsonIgnore]
        [SerializeField]
        protected RigidbodyConstraints? constraints;

        [JsonIgnore]
        [SerializeField]
        protected Vector3? centerOfMass;

        [JsonIgnore]
        [SerializeField]
        protected Vector3? inertiaTensor;

        [JsonIgnore]
        [SerializeField]
        protected Quaternion? inertiaTensorRotation;

        [JsonIgnore]
        [SerializeField]
        protected float? maxAngularVelocity;

        [JsonIgnore]
        [SerializeField]
        protected float? maxDepenetrationVelocity;

        [JsonIgnore]
        [SerializeField]
        protected float? sleepThreshold;

        [JsonProperty("linearVelocity")]
        public Vector3? LinearVelocity {
            get => linearVelocity;
            protected set => linearVelocity = value;
        }

        [JsonProperty("angularVelocity")]
        public Vector3? AngularVelocity {
            get => angularVelocity;
            protected set => angularVelocity = value;
        }

        [JsonProperty("linearDamping")]
        public float? LinearDamping {
            get => linearDamping;
            protected set => linearDamping = value;
        }

        [JsonProperty("angularDamping")]
        public float? AngularDamping {
            get => angularDamping;
            protected set => angularDamping = value;
        }

        [JsonProperty("mass")]
        public float? Mass {
            get => mass;
            protected set => mass = value;
        }

        [JsonProperty("useGravity")]
        public bool? UseGravity {
            get => useGravity;
            protected set => useGravity = value;
        }

        [JsonProperty("isKinematic")]
        public bool? IsKinematic {
            get => isKinematic;
            protected set => isKinematic = value;
        }

        [JsonProperty("interpolation")]
        public RigidbodyInterpolation? Interpolation {
            get => interpolation;
            protected set => interpolation = value;
        }

        [JsonProperty("collisionDetectionMode")]
        public CollisionDetectionMode? CollisionDetectionMode {
            get => collisionDetectionMode;
            protected set => collisionDetectionMode = value;
        }

        [JsonProperty("constraints")]
        public RigidbodyConstraints? Constraints {
            get => constraints;
            protected set => constraints = value;
        }

        [JsonProperty("centerOfMass")]
        public Vector3? CenterOfMass {
            get => centerOfMass;
            protected set => centerOfMass = value;
        }

        [JsonProperty("inertiaTensor")]
        public Vector3? InertiaTensor {
            get => inertiaTensor;
            protected set => inertiaTensor = value;
        }

        [JsonProperty("inertiaTensorRotation")]
        public Quaternion? InertiaTensorRotation {
            get => inertiaTensorRotation;
            protected set => inertiaTensorRotation = value;
        }

        [JsonProperty("maxAngularVelocity")]
        public float? MaxAngularVelocity {
            get => maxAngularVelocity;
            protected set => maxAngularVelocity = value;
        }

        [JsonProperty("maxDepenetrationVelocity")]
        public float? MaxDepenetrationVelocity {
            get => maxDepenetrationVelocity;
            protected set => maxDepenetrationVelocity = value;
        }

        [JsonProperty("sleepThreshold")]
        public float? SleepThreshold {
            get => sleepThreshold;
            protected set => sleepThreshold = value;
        }

        public RigidBodySnapshot()
        {
        }

        public RigidBodySnapshot(T target) : base(target)
        {
        }

        protected RigidBodySnapshot(ComponentSnapshot<T> original) : base(original)
        {
        }

        public RigidBodySnapshot<T> SetLinearVelocity(Vector3? value)
        {
            LinearVelocity = value;
            return this;
        }

        public RigidBodySnapshot<T> SetAngularVelocity(Vector3? value)
        {
            AngularVelocity = value;
            return this;
        }

        public RigidBodySnapshot<T> SetLinearDamping(float? value)
        {
            LinearDamping = value;
            return this;
        }

        public RigidBodySnapshot<T> SetAngularDamping(float? value)
        {
            AngularDamping = value;
            return this;
        }

        public RigidBodySnapshot<T> SetMass(float? value)
        {
            Mass = value;
            return this;
        }

        public RigidBodySnapshot<T> SetUseGravity(bool? value)
        {
            UseGravity = value;
            return this;
        }

        public RigidBodySnapshot<T> SetIsKinematic(bool? value)
        {
            IsKinematic = value;
            return this;
        }

        public RigidBodySnapshot<T> SetInterpolation(RigidbodyInterpolation? value)
        {
            Interpolation = value;
            return this;
        }

        public RigidBodySnapshot<T> SetCollisionDetectionMode(CollisionDetectionMode? value)
        {
            CollisionDetectionMode = value;
            return this;
        }

        public RigidBodySnapshot<T> SetConstraints(RigidbodyConstraints? value)
        {
            Constraints = value;
            return this;
        }

        public RigidBodySnapshot<T> SetCenterOfMass(Vector3? value)
        {
            CenterOfMass = value;
            return this;
        }

        public RigidBodySnapshot<T> SetInertiaTensor(Vector3? value)
        {
            InertiaTensor = value;
            return this;
        }

        public RigidBodySnapshot<T> SetInertiaTensorRotation(Quaternion? value)
        {
            InertiaTensorRotation = value;
            return this;
        }

        public RigidBodySnapshot<T> SetMaxAngularVelocity(float? value)
        {
            MaxAngularVelocity = value;
            return this;
        }

        public RigidBodySnapshot<T> SetMaxDepenetrationVelocity(float? value)
        {
            MaxDepenetrationVelocity = value;
            return this;
        }

        public RigidBodySnapshot<T> SetSleepThreshold(float? value)
        {
            SleepThreshold = value;
            return this;
        }

        protected override void OnRestore(ref T target)
        {
            base.OnRestore(ref target);

            if (linearVelocity.HasValue)
                target.linearVelocity = linearVelocity.Value;

            if (angularVelocity.HasValue)
                target.angularVelocity = angularVelocity.Value;

            if (linearDamping.HasValue)
                target.linearDamping = linearDamping.Value;

            if (angularDamping.HasValue)
                target.angularDamping = angularDamping.Value;

            if (mass.HasValue)
                target.mass = mass.Value;

            if (useGravity.HasValue)
                target.useGravity = useGravity.Value;

            if (isKinematic.HasValue)
                target.isKinematic = isKinematic.Value;

            if (interpolation.HasValue)
                target.interpolation = interpolation.Value;

            if (collisionDetectionMode.HasValue)
                target.collisionDetectionMode = collisionDetectionMode.Value;

            if (constraints.HasValue)
                target.constraints = constraints.Value;

            if (centerOfMass.HasValue)
                target.centerOfMass = centerOfMass.Value;

            if (inertiaTensor.HasValue)
                target.inertiaTensor = inertiaTensor.Value;

            if (inertiaTensorRotation.HasValue)
                target.inertiaTensorRotation = inertiaTensorRotation.Value;

            if (maxAngularVelocity.HasValue)
                target.maxAngularVelocity = maxAngularVelocity.Value;

            if (maxDepenetrationVelocity.HasValue)
                target.maxDepenetrationVelocity = maxDepenetrationVelocity.Value;

            if (sleepThreshold.HasValue)
                target.sleepThreshold = sleepThreshold.Value;
        }

        protected override void OnCapture(T target)
        {
            base.OnCapture(target);

            LinearVelocity = target.linearVelocity;
            AngularVelocity = target.angularVelocity;
            LinearDamping = target.linearDamping;
            AngularDamping = target.angularDamping;
            Mass = target.mass;
            UseGravity = target.useGravity;
            IsKinematic = target.isKinematic;
            Interpolation = target.interpolation;
            CollisionDetectionMode = target.collisionDetectionMode;
            Constraints = target.constraints;
            CenterOfMass = target.centerOfMass;
            InertiaTensor = target.inertiaTensor;
            InertiaTensorRotation = target.inertiaTensorRotation;
            MaxAngularVelocity = target.maxAngularVelocity;
            MaxDepenetrationVelocity = target.maxDepenetrationVelocity;
            SleepThreshold = target.sleepThreshold;
        }

        protected override void OnReset()
        {
            base.OnReset();

            LinearVelocity = null;
            AngularVelocity = null;
            LinearDamping = null;
            AngularDamping = null;
            Mass = null;
            UseGravity = null;
            IsKinematic = null;
            Interpolation = null;
            CollisionDetectionMode = null;
            Constraints = null;
            CenterOfMass = null;
            InertiaTensor = null;
            InertiaTensorRotation = null;
            MaxAngularVelocity = null;
            MaxDepenetrationVelocity = null;
            SleepThreshold = null;
        }
    }

    [Serializable]
    [SerializationDescriptor("RigidBodySnapshot", "93662e04-2ce4-4ea1-8761-60efc7d50534")]
    public record RigidBodySnapshot : RigidBodySnapshot<Rigidbody>
    {
        public RigidBodySnapshot()
        {
        }

        public RigidBodySnapshot(Rigidbody target) : base(target)
        {
        }

        protected RigidBodySnapshot(RigidBodySnapshot<Rigidbody> original) : base(original)
        {
        }

        protected RigidBodySnapshot(ComponentSnapshot<Rigidbody> original) : base(original)
        {
        }
    }
}
