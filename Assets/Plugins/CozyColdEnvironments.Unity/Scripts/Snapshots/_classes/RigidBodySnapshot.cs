using System;
using System.Runtime.Serialization;
using CCEnvs.Attributes.Serialization;
using CommunityToolkit.Diagnostics;
using Newtonsoft.Json;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable, DataContract]
    public record RigidbodySnapshot<T> : ComponentSnapshot<T>
        where T : Rigidbody
    {
        [SerializeField]
        protected Vector3? linearVelocity;

        [SerializeField]
        protected Vector3? angularVelocity;

        [SerializeField]
        protected float? linearDamping;

        [SerializeField]
        protected float? angularDamping;

        [SerializeField]
        protected float? mass;

        [SerializeField]
        protected bool? useGravity;

        [SerializeField]
        protected bool? isKinematic;

        [SerializeField]
        protected RigidbodyInterpolation? interpolation;

        [SerializeField]
        protected CollisionDetectionMode? collisionDetectionMode;

        [SerializeField]
        protected RigidbodyConstraints? constraints;

        [SerializeField]
        protected Vector3? centerOfMass;

        [SerializeField]
        protected Vector3? inertiaTensor;

        [SerializeField]
        protected Quaternion? inertiaTensorRotation;

        [SerializeField]
        protected float? maxAngularVelocity;

        [SerializeField]
        protected float? maxDepenetrationVelocity;

        [SerializeField]
        protected float? sleepThreshold;

        [SerializeField]
        protected bool? detectCollisions;

        [SerializeField]
        protected int? solverIterations;

        [SerializeField]
        protected int? solverVelocityIterations;

        [SerializeField]
        protected LayerMask? includeLayers;

        [SerializeField]
        protected LayerMask? excludeLayers;

        [SerializeField]
        protected bool? automaticCenterOfMass;

        [SerializeField]
        protected bool? automaticInertiaTensor;

        [SerializeField]
        protected float? maxLinearVelocity;

        [JsonProperty("linearVelocity", NullValueHandling = NullValueHandling.Ignore)]
        public Vector3? LinearVelocity {
            get => linearVelocity;
            protected set => linearVelocity = value;
        }

        [JsonProperty("angularVelocity", NullValueHandling = NullValueHandling.Ignore)]
        public Vector3? AngularVelocity {
            get => angularVelocity;
            protected set => angularVelocity = value;
        }

        [JsonProperty("linearDamping", NullValueHandling = NullValueHandling.Ignore)]
        public float? LinearDamping {
            get => linearDamping;
            protected set => linearDamping = value;
        }

        [JsonProperty("angularDamping", NullValueHandling = NullValueHandling.Ignore)]
        public float? AngularDamping {
            get => angularDamping;
            protected set => angularDamping = value;
        }

        [JsonProperty("mass", NullValueHandling = NullValueHandling.Ignore)]
        public float? Mass {
            get => mass;
            protected set => mass = value;
        }

        [JsonProperty("useGravity", NullValueHandling = NullValueHandling.Ignore)]
        public bool? UseGravity {
            get => useGravity;
            protected set => useGravity = value;
        }

        [JsonProperty("isKinematic", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsKinematic {
            get => isKinematic;
            protected set => isKinematic = value;
        }

        [JsonProperty("interpolation", NullValueHandling = NullValueHandling.Ignore)]
        public RigidbodyInterpolation? Interpolation {
            get => interpolation;
            protected set => interpolation = value;
        }

        [JsonProperty("collisionDetectionMode", NullValueHandling = NullValueHandling.Ignore)]
        public CollisionDetectionMode? CollisionDetectionMode {
            get => collisionDetectionMode;
            protected set => collisionDetectionMode = value;
        }

        [JsonProperty("constraints", NullValueHandling = NullValueHandling.Ignore)]
        public RigidbodyConstraints? Constraints {
            get => constraints;
            protected set => constraints = value;
        }

        [JsonProperty("centerOfMass", NullValueHandling = NullValueHandling.Ignore)]
        public Vector3? CenterOfMass {
            get => centerOfMass;
            protected set => centerOfMass = value;
        }

        [JsonProperty("inertiaTensor", NullValueHandling = NullValueHandling.Ignore)]
        public Vector3? InertiaTensor {
            get => inertiaTensor;
            protected set => inertiaTensor = value;
        }

        [JsonProperty("inertiaTensorRotation", NullValueHandling = NullValueHandling.Ignore)]
        public Quaternion? InertiaTensorRotation {
            get => inertiaTensorRotation;
            protected set => inertiaTensorRotation = value;
        }

        [JsonProperty("maxAngularVelocity", NullValueHandling = NullValueHandling.Ignore)]
        public float? MaxAngularVelocity {
            get => maxAngularVelocity;
            protected set => maxAngularVelocity = value;
        }

        [JsonProperty("maxDepenetrationVelocity", NullValueHandling = NullValueHandling.Ignore)]
        public float? MaxDepenetrationVelocity {
            get => maxDepenetrationVelocity;
            protected set => maxDepenetrationVelocity = value;
        }

        [JsonProperty("sleepThreshold", NullValueHandling = NullValueHandling.Ignore)]
        public float? SleepThreshold {
            get => sleepThreshold;
            protected set => sleepThreshold = value;
        }

        [JsonProperty("detectCollisions", NullValueHandling = NullValueHandling.Ignore)]
        public bool? DetectCollisions {
            get => detectCollisions;
            protected set => detectCollisions = value;
        }

        [JsonProperty("solverIterations", NullValueHandling = NullValueHandling.Ignore)]
        public int? SolverIterations {
            get => solverIterations;
            protected set => solverIterations = value;
        }

        [JsonProperty("solverVelocityIterations", NullValueHandling = NullValueHandling.Ignore)]
        public int? SolverVelocityIterations {
            get => solverVelocityIterations;
            protected set => solverVelocityIterations = value;
        }

        [JsonProperty("includeLayers", NullValueHandling = NullValueHandling.Ignore)]
        public LayerMask? IncludeLayers {
            get => includeLayers;
            protected set => includeLayers = value;
        }

        [JsonProperty("excludeLayers", NullValueHandling = NullValueHandling.Ignore)]
        public LayerMask? ExcludeLayers {
            get => excludeLayers;
            protected set => excludeLayers = value;
        }

        [JsonProperty("automaticCenterOfMass", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AutomaticCenterOfMass {
            get => automaticCenterOfMass;
            protected set => automaticCenterOfMass = value;
        }

        [JsonProperty("automaticInertiaTensor", NullValueHandling = NullValueHandling.Ignore)]
        public bool? AutomaticInertiaTensor {
            get => automaticInertiaTensor;
            protected set => automaticInertiaTensor = value;
        }

        [JsonProperty("maxLinearVelocity", NullValueHandling = NullValueHandling.Ignore)]
        public float? MaxLinearVelocity {
            get => maxLinearVelocity;
            protected set => maxLinearVelocity = value;
        }

        public RigidbodySnapshot()
        {
        }

        public RigidbodySnapshot(T target) : base(target)
        {
        }

        protected RigidbodySnapshot(ComponentSnapshot<T> original) : base(original)
        {
        }

        private int configureDepth;
        public RigidbodySnapshot<T> Configure(Action<RigidbodySnapshot<T>> configurer)
        {
            Guard.IsNotNull(configurer, nameof(configurer));

            if (configureDepth != 0)
                throw new InvalidOperationException($"Cannot call {nameof(Configure)} twice");

            configureDepth++;

            try
            {
                configurer(this);
            }
            finally
            {
                configureDepth--;
            }

            return this;
        }

        public RigidbodySnapshot<T> SetLinearVelocity(Vector3? value)
        {
            LinearVelocity = value;
            return this;
        }

        public RigidbodySnapshot<T> SetAngularVelocity(Vector3? value)
        {
            AngularVelocity = value;
            return this;
        }

        public RigidbodySnapshot<T> SetLinearDamping(float? value)
        {
            LinearDamping = value;
            return this;
        }

        public RigidbodySnapshot<T> SetAngularDamping(float? value)
        {
            AngularDamping = value;
            return this;
        }

        public RigidbodySnapshot<T> SetMass(float? value)
        {
            Mass = value;
            return this;
        }

        public RigidbodySnapshot<T> SetUseGravity(bool? value)
        {
            UseGravity = value;
            return this;
        }

        public RigidbodySnapshot<T> SetIsKinematic(bool? value)
        {
            IsKinematic = value;
            return this;
        }

        public RigidbodySnapshot<T> SetInterpolation(RigidbodyInterpolation? value)
        {
            Interpolation = value;
            return this;
        }

        public RigidbodySnapshot<T> SetCollisionDetectionMode(CollisionDetectionMode? value)
        {
            CollisionDetectionMode = value;
            return this;
        }

        public RigidbodySnapshot<T> SetConstraints(RigidbodyConstraints? value)
        {
            Constraints = value;
            return this;
        }

        public RigidbodySnapshot<T> SetCenterOfMass(Vector3? value)
        {
            CenterOfMass = value;
            return this;
        }

        public RigidbodySnapshot<T> SetInertiaTensor(Vector3? value)
        {
            InertiaTensor = value;
            return this;
        }

        public RigidbodySnapshot<T> SetInertiaTensorRotation(Quaternion? value)
        {
            InertiaTensorRotation = value;
            return this;
        }

        public RigidbodySnapshot<T> SetMaxAngularVelocity(float? value)
        {
            MaxAngularVelocity = value;
            return this;
        }

        public RigidbodySnapshot<T> SetMaxDepenetrationVelocity(float? value)
        {
            MaxDepenetrationVelocity = value;
            return this;
        }

        public RigidbodySnapshot<T> SetSleepThreshold(float? value)
        {
            SleepThreshold = value;
            return this;
        }

        public RigidbodySnapshot<T> SetDetectCollisions(bool? value)
        {
            DetectCollisions = value;
            return this;
        }

        public RigidbodySnapshot<T> SetSolverIterations(int? value)
        {
            SolverIterations = value;
            return this;
        }

        public RigidbodySnapshot<T> SetSolverVelocityIterations(int? value)
        {
            SolverVelocityIterations = value;
            return this;
        }

        public RigidbodySnapshot<T> SetIncludeLayers(LayerMask? value)
        {
            IncludeLayers = value;
            return this;
        }

        public RigidbodySnapshot<T> SetExcludeLayers(LayerMask? value)
        {
            ExcludeLayers = value;
            return this;
        }

        public RigidbodySnapshot<T> SetAutomaticCenterOfMass(bool? value)
        {
            AutomaticCenterOfMass = value;
            return this;
        }

        public RigidbodySnapshot<T> SetAutomaticInertiaTensor(bool? value)
        {
            AutomaticInertiaTensor = value;
            return this;
        }

        public RigidbodySnapshot<T> SetMaxLinearVelocity(float? value)
        {
            MaxLinearVelocity = value;
            return this;
        }

        protected override void OnRestore(ref T target)
        {
            base.OnRestore(ref target);

            if (linearVelocity.HasValue)
                target.linearVelocity = linearVelocity.Value;

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

            if (angularVelocity.HasValue && !target.isKinematic)
                target.angularVelocity = angularVelocity.Value;

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

            if (detectCollisions.HasValue)
                target.detectCollisions = detectCollisions.Value;

            if (solverIterations.HasValue)
                target.solverIterations = solverIterations.Value;

            if (solverVelocityIterations.HasValue)
                target.solverVelocityIterations = solverVelocityIterations.Value;

            if (includeLayers.HasValue)
                target.includeLayers = includeLayers.Value;

            if (excludeLayers.HasValue)
                target.excludeLayers = excludeLayers.Value;

            if (automaticCenterOfMass.HasValue)
                target.automaticCenterOfMass = automaticCenterOfMass.Value;

            if (automaticInertiaTensor.HasValue)
                target.automaticInertiaTensor = automaticInertiaTensor.Value;

            if (maxLinearVelocity.HasValue)
                target.maxLinearVelocity = maxLinearVelocity.Value;
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
            DetectCollisions = target.detectCollisions;
            SolverIterations = target.solverIterations;
            SolverVelocityIterations = target.solverVelocityIterations;
            IncludeLayers = target.includeLayers;
            ExcludeLayers = target.excludeLayers;
            AutomaticCenterOfMass = target.automaticCenterOfMass;
            AutomaticInertiaTensor = target.automaticInertiaTensor;
            MaxLinearVelocity = target.maxLinearVelocity;
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
            DetectCollisions = null;
            SolverIterations = null;
            SolverVelocityIterations = null;
            IncludeLayers = null;
            ExcludeLayers = null;
            AutomaticCenterOfMass = null;
            AutomaticInertiaTensor = null;
            MaxLinearVelocity = null;
        }
    }

    [Serializable]
    [SerializationDescriptor("RigidBodySnapshot", "93662e04-2ce4-4ea1-8761-60efc7d50534")]
    public record RigidbodySnapshot : RigidbodySnapshot<Rigidbody>
    {
        public RigidbodySnapshot()
        {
        }

        public RigidbodySnapshot(Rigidbody target) : base(target)
        {
        }

        protected RigidbodySnapshot(RigidbodySnapshot<Rigidbody> original) : base(original)
        {
        }

        protected RigidbodySnapshot(ComponentSnapshot<Rigidbody> original) : base(original)
        {
        }
    }
}