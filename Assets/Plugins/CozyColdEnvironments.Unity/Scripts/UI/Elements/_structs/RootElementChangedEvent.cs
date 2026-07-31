using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public readonly struct RootElementChangedEvent : IEquatable<RootElementChangedEvent>
    {
        public VisualElement? Previous { get; }
        public VisualElement? Current { get; }

        public RootElementChangedEvent(VisualElement? previous, VisualElement? current)
        {
            Previous = previous;
            Current = current;
        }

        public void Deconstruct(out VisualElement? previous, out VisualElement? current)
        {
            previous = Previous;
            current = Current;
        }

        public static bool operator ==(RootElementChangedEvent left, RootElementChangedEvent right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RootElementChangedEvent left, RootElementChangedEvent right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is RootElementChangedEvent @event && Equals(@event);
        }

        public bool Equals(RootElementChangedEvent other)
        {
            return EqualityComparer<VisualElement?>.Default.Equals(Previous, other.Previous) &&
                   EqualityComparer<VisualElement?>.Default.Equals(Current, other.Current);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Previous, Current);
        }
    }
}
