using System;
using CCEnvs.Attributes.Serialization;
using Newtonsoft.Json;
using UnityEngine;

#nullable enable
namespace CCEnvs.Unity.Snapshots
{
    [Serializable]
    public record ColliderSnapshot<T> : ComponentSnapshot<T>
        where T : Collider
    {
        [JsonIgnore]
        [SerializeField]
        protected bool? enabled;

        [JsonIgnore]
        [SerializeField]
        protected bool? isTrigger;

        [JsonIgnore]
        [SerializeField]
        protected float? contactOffset;

        [JsonIgnore]
        [SerializeField]
        protected bool? hasModifiableContacts;

        [JsonIgnore]
        [SerializeField]
        protected bool? providesContacts;

        [JsonIgnore]
        [SerializeField]
        protected int? layerOverridePriority;

        [JsonIgnore]
        [SerializeField]
        protected LayerMask? includeLayers;

        [JsonIgnore]
        [SerializeField]
        protected LayerMask? excludeLayers;

        [JsonIgnore]
        [SerializeField]
        protected PhysicsMaterial? sharedMaterial;

        [JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Enabled {
            get => enabled;
            protected set => enabled = value;
        }

        [JsonProperty("isTrigger", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsTrigger {
            get => isTrigger;
            protected set => isTrigger = value;
        }

        [JsonProperty("contactOffset", NullValueHandling = NullValueHandling.Ignore)]
        public float? ContactOffset {
            get => contactOffset;
            protected set => contactOffset = value;
        }

        [JsonProperty("hasModifiableContacts", NullValueHandling = NullValueHandling.Ignore)]
        public bool? HasModifiableContacts {
            get => hasModifiableContacts;
            protected set => hasModifiableContacts = value;
        }

        [JsonProperty("providesContacts", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ProvidesContacts {
            get => providesContacts;
            protected set => providesContacts = value;
        }

        [JsonProperty("layerOverridePriority", NullValueHandling = NullValueHandling.Ignore)]
        public int? LayerOverridePriority {
            get => layerOverridePriority;
            protected set => layerOverridePriority = value;
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

        [JsonProperty("sharedMaterial", NullValueHandling = NullValueHandling.Ignore)]
        public PhysicsMaterial? SharedMaterial {
            get => sharedMaterial;
            protected set => sharedMaterial = value;
        }

        public ColliderSnapshot()
        {
        }

        public ColliderSnapshot(T target) : base(target)
        {
        }

        protected ColliderSnapshot(ComponentSnapshot<T> original) : base(original)
        {
        }

        public ColliderSnapshot<T> SetEnabled(bool? value)
        {
            Enabled = value;
            return this;
        }

        public ColliderSnapshot<T> SetIsTrigger(bool? value)
        {
            IsTrigger = value;
            return this;
        }

        public ColliderSnapshot<T> SetContactOffset(float? value)
        {
            ContactOffset = value;
            return this;
        }

        public ColliderSnapshot<T> SetHasModifiableContacts(bool? value)
        {
            HasModifiableContacts = value;
            return this;
        }

        public ColliderSnapshot<T> SetProvidesContacts(bool? value)
        {
            ProvidesContacts = value;
            return this;
        }

        public ColliderSnapshot<T> SetLayerOverridePriority(int? value)
        {
            LayerOverridePriority = value;
            return this;
        }

        public ColliderSnapshot<T> SetIncludeLayers(LayerMask? value)
        {
            IncludeLayers = value;
            return this;
        }

        public ColliderSnapshot<T> SetExcludeLayers(LayerMask? value)
        {
            ExcludeLayers = value;
            return this;
        }

        public ColliderSnapshot<T> SetSharedMaterial(PhysicsMaterial? value)
        {
            SharedMaterial = value;
            return this;
        }

        protected override void OnRestore(ref T target)
        {
            base.OnRestore(ref target);

            if (enabled.HasValue)
                target.enabled = enabled.Value;

            if (isTrigger.HasValue)
                target.isTrigger = isTrigger.Value;

            if (contactOffset.HasValue)
                target.contactOffset = contactOffset.Value;

            if (hasModifiableContacts.HasValue)
                target.hasModifiableContacts = hasModifiableContacts.Value;

            if (providesContacts.HasValue)
                target.providesContacts = providesContacts.Value;

            if (layerOverridePriority.HasValue)
                target.layerOverridePriority = layerOverridePriority.Value;

            if (includeLayers.HasValue)
                target.includeLayers = includeLayers.Value;

            if (excludeLayers.HasValue)
                target.excludeLayers = excludeLayers.Value;

            if (sharedMaterial)
                target.sharedMaterial = sharedMaterial;
        }

        protected override void OnCapture(T target)
        {
            base.OnCapture(target);

            Enabled = target.enabled;
            IsTrigger = target.isTrigger;
            ContactOffset = target.contactOffset;
            HasModifiableContacts = target.hasModifiableContacts;
            ProvidesContacts = target.providesContacts;
            LayerOverridePriority = target.layerOverridePriority;
            IncludeLayers = target.includeLayers;
            ExcludeLayers = target.excludeLayers;
            SharedMaterial = target.sharedMaterial;
        }

        protected override void OnReset()
        {
            base.OnReset();

            Enabled = null;
            IsTrigger = null;
            ContactOffset = null;
            HasModifiableContacts = null;
            ProvidesContacts = null;
            LayerOverridePriority = null;
            IncludeLayers = null;
            ExcludeLayers = null;
            SharedMaterial = null;
        }
    }

    [Serializable]
    [SerializationDescriptor("ColliderSnapshot", "feb33794-8105-4bc5-a312-05acec5b19d6")]
    public record ColliderSnapshot : ColliderSnapshot<Collider>
    {
        public ColliderSnapshot()
        {
        }

        public ColliderSnapshot(Collider target) : base(target)
        {
        }

        protected ColliderSnapshot(ColliderSnapshot<Collider> original) : base(original)
        {
        }

        protected ColliderSnapshot(ComponentSnapshot<Collider> original) : base(original)
        {
        }
    }
}