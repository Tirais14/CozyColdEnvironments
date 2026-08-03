using CommunityToolkit.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public class BeginDragEvent
    {
        public VisualElement Source { get; private set; } = null!;
        public VisualElement? Target { get; private set; } = null!;

        public GameObject SourceGameObject { get; private set; } = null!;
        public GameObject? TargetGameObject { get; private set; } = null!;

        public PointerEventSnapshot Info { get; private set; } = null!;

        public BeginDragEvent(
            VisualElement source,
            GameObject sourceGameObject,
            PointerEventSnapshot ev
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

        public BeginDragEvent() { }

        public BeginDragEvent SetSource(VisualElement source, GameObject sourceGameObject)
        {
            Guard.IsNotNull(source);
            CC.Guard.IsNotNull(sourceGameObject, nameof(sourceGameObject));
            Source = source;
            SourceGameObject = sourceGameObject;
            return this;
        }

        public BeginDragEvent SetTarget(VisualElement? target, GameObject? targetGameObject)
        {
            Target = target.IfNull(Source);
            TargetGameObject = targetGameObject.IfNull(targetGameObject);
            return this;
        }

        public BeginDragEvent SetInfo(PointerEventSnapshot info)
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
