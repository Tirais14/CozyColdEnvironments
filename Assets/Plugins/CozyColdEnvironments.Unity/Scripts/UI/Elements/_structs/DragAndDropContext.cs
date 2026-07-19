using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public static class DragAndDropContext
    {
        public static DragAndDropContext<TEvent> Create<TEvent>(
            VisualElement source,
            GameObject sourceGameObject,
            TEvent ev
            )
            where TEvent : EventBase<TEvent>, new()
        {
            return new DragAndDropContext<TEvent>(
                source,
                sourceGameObject,
                ev
                );
        }
    }

    public readonly struct DragAndDropContext<TEvent> : IEquatable<DragAndDropContext<TEvent>>

        where TEvent : EventBase<TEvent>, new()
    {
        public VisualElement Source { get; }

        public GameObject SourceGameObject { get; }

        public TEvent Event { get; }

        public DragAndDropContext(
            VisualElement source,
            GameObject sourceGameObject,
            TEvent ev
            )
        {
            Guard.IsNotNull(source);
            CC.Guard.IsNotNull(sourceGameObject, nameof(sourceGameObject));
            CC.Guard.IsNotNull(ev, nameof(ev));

            Source = source;
            SourceGameObject = sourceGameObject;
            Event = ev;
        }

        public static bool operator ==(in DragAndDropContext<TEvent> left, in DragAndDropContext<TEvent> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(in DragAndDropContext<TEvent> left, in DragAndDropContext<TEvent> right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is DragAndDropContext<TEvent> context && Equals(context);
        }

        public bool Equals(DragAndDropContext<TEvent> other)
        {
            return EqualityComparer<VisualElement>.Default.Equals(Source, other.Source) &&
                   EqualityComparer<GameObject>.Default.Equals(SourceGameObject, other.SourceGameObject) &&
                   EqualityComparer<TEvent>.Default.Equals(Event, other.Event);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Source, SourceGameObject, Event);
        }

        public override string ToString()
        {
            return ToStringBuilder.CreatePooled()
                .AddProperty(nameof(Source), Source)
                .AddProperty(nameof(SourceGameObject), SourceGameObject)
                .AddProperty(nameof(Event), Event)
                .ToStringAndDispose();
        }
    }
}
