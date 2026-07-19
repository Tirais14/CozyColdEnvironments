using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public readonly struct DropContext
    {
        public VisualElement Source { get; }

        public GameObject SourceGameObject { get; }
        public GameObject TargetGameObject { get; }

        public DropContext(
            VisualElement source,
            GameObject sourceGameObject,
            GameObject targetGameObject
            )
        {
            Source = source;
            SourceGameObject = sourceGameObject;
            TargetGameObject = targetGameObject;
        }

        public static DropContext Create<TEvent>(
            DragContext<TEvent> dragContext,
            GameObject targetGameObject
            )
            where TEvent : EventBase<TEvent>, new()
        {
            return new DropContext(
                dragContext.Source,
                dragContext.SourceGameObject,
                targetGameObject
                );
        }

        public override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddProperty()
        }
    }
}
