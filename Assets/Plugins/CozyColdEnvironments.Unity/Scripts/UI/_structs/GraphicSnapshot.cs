using CCEnvs.Attributes.Serialization;
using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.UI;

#nullable enable
namespace CCEnvs.Unity.Snapshots.UI
{
    [Serializable]
    public record GraphicSnapshot<T> : BehaviourSnapshot<T>
        where T : Graphic
    {
        [JsonProperty("color")]
        public Color? Color { get; set; }

        [JsonProperty("raycastTarget")]
        public bool? RaycastTarget { get; set; }

        public GraphicSnapshot()
        {
        }

        public GraphicSnapshot(T target)
            :
            base(target)
        {
        }

        protected override void OnRestore(ref T target)
        {
            base.OnRestore(ref target);

            if (Color.HasValue)
                target.color = Color.Value;

            if (RaycastTarget.HasValue)
                target.raycastTarget = RaycastTarget.Value;
        }

        protected override void OnCapture(T target)
        {
            base.OnCapture(target);

            Color = target.color;
            RaycastTarget = target.raycastTarget;
        }

        protected override void OnReset()
        {
            base.OnReset();

            Color = default;
            RaycastTarget = default;
        }
    }

    [Serializable]
    [TypeSerializationDescriptor("GraphicSnapshot", "ade0f9d1-6ddc-487f-abd0-ab8ac2e88d4e")]
    public record GraphicSnapshot : GraphicSnapshot<Graphic>
    {
        public GraphicSnapshot()
        {
        }

        public GraphicSnapshot(Graphic target) : base(target)
        {
        }

        protected GraphicSnapshot(GraphicSnapshot<Graphic> original) : base(original)
        {
        }
    }
}
