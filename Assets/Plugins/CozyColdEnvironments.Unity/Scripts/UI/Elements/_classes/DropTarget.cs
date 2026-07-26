using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public sealed class DropTarget
    {
        public string Tag => GameObject.tag;

        public int Layer => GameObject.layer;

        public GameObject GameObject { get; }

        public DropTarget(GameObject gameObject)
        {
            GameObject = gameObject;
        }

        public override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddProperty(nameof(Tag), Tag)
                .AddProperty(nameof(Layer), LayerMask.LayerToName(Layer))
                .AddProperty(nameof(GameObject), GameObject)
                .ToStringAndDispose();
        }
    }
}
