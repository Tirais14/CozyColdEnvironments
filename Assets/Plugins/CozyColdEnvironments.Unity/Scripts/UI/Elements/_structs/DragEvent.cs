using CommunityToolkit.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public sealed class DragEvent
    {
        public VisualElement Source { get; private set; }
        public VisualElement Target { get; private set; }

        public GameObject SourceGameObject { get; private set; }
        public GameObject TargetGameObject { get; private set; }

        public IPointerEvent Event { get; set; }

        public DragEvent(
            VisualElement source,
            GameObject sourceGameObject
            )
        {
            Guard.IsNotNull(source);
            CC.Guard.IsNotNull(sourceGameObject, nameof(sourceGameObject));

            Source = source;
            Target = source;
            SourceGameObject = sourceGameObject;
            TargetGameObject = sourceGameObject;
        }

        public DragEvent SetSource(VisualElement source, GameObject sourceGameObject)
        {
            Guard.IsNotNull(source);
            CC.Guard.IsNotNull(sourceGameObject, nameof(sourceGameObject));
            Source = source;
            SourceGameObject = sourceGameObject;
            return this;
        }

        public DragEvent SetTarget(VisualElement target, GameObject targetGameObject)
        {
            Guard.IsNotNull(target);
            CC.Guard.IsNotNull(targetGameObject, nameof(targetGameObject));
            Target = target;
            TargetGameObject = targetGameObject;
            return this;
        }

        public override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddProperty(nameof(Source), Source)
                .AddProperty(nameof(Target), Target)
                .AddProperty(nameof(SourceGameObject), SourceGameObject)
                .AddProperty(nameof(TargetGameObject), TargetGameObject)
                .AddProperty(nameof(Event), Event)
                .ToStringAndDispose();
        }
    }
}
