using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public readonly struct DragContext : IEquatable<DragContext>
    {
        public VisualElement Source { get; }

        public GameObject SourceGameObject { get; }

        public IPointerEvent Event { get; }

        public DragContext(
            VisualElement source,
            GameObject sourceGameObject,
            IPointerEvent ev
            )
        {
            Guard.IsNotNull(source);
            CC.Guard.IsNotNull(sourceGameObject, nameof(source));
            Guard.IsNotNull(ev);

            Source = source;
            SourceGameObject = sourceGameObject;
            Event = ev;
        }

        public static DragContext<TEvent> Create<TEvent>(
            VisualElement source,
            GameObject sourceGameObject,
            TEvent ev
            )
            where TEvent : EventBase<TEvent>, IPointerEvent, new()
        {
            return new DragContext<TEvent>(
                source,
                sourceGameObject,
                ev
                );
        }

        public static bool operator ==(in DragContext left, in DragContext right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(in DragContext left, in DragContext right)
        {
            return !(left == right);
        }

        public DragContext<TEvent> Convert<TEvent>()
            where TEvent : EventBase<TEvent>, IPointerEvent, new()
        {
            return new DragContext<TEvent>(
                Source,
                SourceGameObject,
                Event.CastTo<TEvent>()
                );
        }

        public override bool Equals(object? obj)
        {
            return obj is DragContext context && Equals(context);
        }

        public bool Equals(DragContext other)
        {
            return EqualityComparer<VisualElement>.Default.Equals(Source, other.Source) &&
                   EqualityComparer<GameObject>.Default.Equals(SourceGameObject, other.SourceGameObject) &&
                   EqualityComparer<IPointerEvent>.Default.Equals(Event, other.Event);
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

    public readonly struct DragContext<TEvent> : IEquatable<DragContext<TEvent>>

        where TEvent : EventBase<TEvent>, IPointerEvent, new()
    {
        public VisualElement Source { get; }

        public GameObject SourceGameObject { get; }

        public TEvent Event { get; }

        public DragContext(
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

        public static implicit operator DragContext(DragContext<TEvent> instance)
        {
            return instance.AsUntyped();
        }

        public static bool operator ==(in DragContext<TEvent> left, in DragContext<TEvent> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(in DragContext<TEvent> left, in DragContext<TEvent> right)
        {
            return !(left == right);
        }

        public DragContext AsUntyped()
        {
            return new DragContext(
                Source,
                SourceGameObject,
                Event
                );
        }

        public override bool Equals(object? obj)
        {
            return obj is DragContext<TEvent> context && Equals(context);
        }

        public bool Equals(DragContext<TEvent> other)
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
