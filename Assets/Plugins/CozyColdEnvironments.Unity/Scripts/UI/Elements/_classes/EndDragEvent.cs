using CommunityToolkit.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public class EndDragEvent
    {
        public VisualElement Source { get; private set; } = null!;
        public VisualElement? Target { get; private set; } = null!;

        public GameObject SourceGameObject { get; private set; } = null!;
        public GameObject? TargetGameObject { get; private set; } = null!;

        public IPointerEvent Info { get; private set; } = null!;

        public EndDragEvent(
            VisualElement source,
            GameObject sourceGameObject,
            IPointerEvent ev
            )
        {
            Guard.IsNotNull(source);
            CC.Guard.IsNotNull(sourceGameObject, nameof(sourceGameObject));

            Source = source;
            Target = source;
            SourceGameObject = sourceGameObject;
            TargetGameObject = sourceGameObject;
            Info = ev;
        }

        public EndDragEvent() { }

        public EndDragEvent SetSource(VisualElement source, GameObject sourceGameObject)
        {
            Guard.IsNotNull(source);
            CC.Guard.IsNotNull(sourceGameObject, nameof(sourceGameObject));
            Source = source;
            SourceGameObject = sourceGameObject;
            return this;
        }

        public EndDragEvent SetTarget(VisualElement? target, GameObject? targetGameObject)
        {
            Target = target.IfNull(Source);
            TargetGameObject = targetGameObject.IfNull(targetGameObject);
            return this;
        }

        public EndDragEvent SetInfo(IPointerEvent info)
        {
            Info = info;
            return this;
        }

        public override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddProperty(nameof(Source), Source)
                .AddProperty(nameof(Target), Target)
                .AddProperty(nameof(SourceGameObject), SourceGameObject)
                .AddProperty(nameof(TargetGameObject), TargetGameObject)
                .AddProperty(nameof(Info), Info)
                .ToStringAndDispose();
        }
    }
}
