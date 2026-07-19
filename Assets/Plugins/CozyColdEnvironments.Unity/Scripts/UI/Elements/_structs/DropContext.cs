using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public readonly struct DropContext : IEquatable<DropContext>
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
            Guard.IsNotNull(source);
            CC.Guard.IsNotNull(sourceGameObject, nameof(sourceGameObject));
            CC.Guard.IsNotNull(targetGameObject, nameof(targetGameObject));

            Source = source;
            SourceGameObject = sourceGameObject;
            TargetGameObject = targetGameObject;
        }

        public static DropContext Create(
            DragContext dragContext,
            GameObject targetGameObject
            )
        {
            return new DropContext(
                dragContext.Source,
                dragContext.SourceGameObject,
                targetGameObject
                );
        }

        public static bool operator ==(in DropContext left, in DropContext right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(in DropContext left, in DropContext right)
        {
            return !(left == right);
        }

        public static DropContext Create<TEvent>(
            DragContext<TEvent> dragContext,
            GameObject targetGameObject
            )
            where TEvent : EventBase<TEvent>, IPointerEvent, new()
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
                .AddProperty(nameof(Source), Source)
                .AddProperty(nameof(SourceGameObject), Source)
                .AddProperty(nameof(TargetGameObject), TargetGameObject)
                .ToStringAndDispose();
        }

        public override bool Equals(object? obj)
        {
            return obj is DropContext context && Equals(context);
        }

        public bool Equals(DropContext other)
        {
            return EqualityComparer<VisualElement>.Default.Equals(Source, other.Source) &&
                   EqualityComparer<GameObject>.Default.Equals(SourceGameObject, other.SourceGameObject) &&
                   EqualityComparer<GameObject>.Default.Equals(TargetGameObject, other.TargetGameObject);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Source, SourceGameObject, TargetGameObject);
        }
    }
}
