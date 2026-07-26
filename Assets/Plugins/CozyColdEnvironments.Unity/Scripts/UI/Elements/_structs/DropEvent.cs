using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public readonly struct DropEvent : IEquatable<DropEvent>
    {
        public VisualElement Source { get; }
        public VisualElement Target { get; }

        public GameObject SourceGameObject { get; }
        public GameObject TargetGameObject { get; }

        public DropEvent(
            VisualElement source,
            VisualElement? target,
            GameObject sourceGameObject,
            GameObject? targetGameObject
            )
        {
            Guard.IsNotNull(source);
            Guard.IsNotNull(target);
            CC.Guard.IsNotNull(sourceGameObject, nameof(sourceGameObject));
            CC.Guard.IsNotNull(targetGameObject, nameof(targetGameObject));

            Source = source;
            Target = target.IfNull(source);
            SourceGameObject = sourceGameObject;
            TargetGameObject = targetGameObject.IfNull(sourceGameObject);
        }

        public static bool operator ==(in DropEvent left, in DropEvent right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(in DropEvent left, in DropEvent right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is DropEvent context && Equals(context);
        }

        public bool Equals(DropEvent other)
        {
            return EqualityComparer<VisualElement>.Default.Equals(Source, other.Source) &&
                   EqualityComparer<VisualElement>.Default.Equals(Target, other.Target) &&
                   EqualityComparer<GameObject>.Default.Equals(SourceGameObject, other.SourceGameObject) &&
                   EqualityComparer<GameObject>.Default.Equals(TargetGameObject, other.TargetGameObject);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Source, Target, SourceGameObject, TargetGameObject);
        }

        public override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddProperty(nameof(Source), Source)
                .AddProperty(nameof(Target), Target)
                .AddProperty(nameof(SourceGameObject), Source)
                .AddProperty(nameof(TargetGameObject), TargetGameObject)
                .ToStringAndDispose();
        }
    }
}
