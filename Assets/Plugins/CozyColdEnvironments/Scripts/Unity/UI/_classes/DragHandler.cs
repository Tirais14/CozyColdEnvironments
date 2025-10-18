using CCEnvs.Unity.Components;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public class DragHandler
        : CCBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        public event DragAndDropAction? OnBeginDrag;
        public event DragAndDropAction? OnDrag;
        public event DragAndDropAction? OnEndDrag;

        public IObservable<PointerEventData> OnBeginDragRx { get; private set; } = new Subject<PointerEventData>();
        public IObservable<PointerEventData> OnDragRx { get; private set; } = new Subject<PointerEventData>();
        public IObservable<PointerEventData> OnEndDragRx { get; private set; } = new Subject<PointerEventData>();

        public static bool HasOn(GameObject gameObject)
        {
            CC.Guard.NullArgument(gameObject, nameof(gameObject));

            return gameObject.GetComponent<DragHandler>();
        }
        public static bool HasOn(Component component)
        {
            CC.Guard.NullArgument(component, nameof(component));

            return HasOn(component.gameObject);
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            OnBeginDrag?.Invoke(eventData);
            ((Subject<PointerEventData>)OnBeginDragRx).OnNext(eventData);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            OnDrag?.Invoke(eventData);
            ((Subject<PointerEventData>)OnDragRx).OnNext(eventData);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            OnEndDrag?.Invoke(eventData);
            ((Subject<PointerEventData>)OnEndDragRx).OnNext(eventData);
        }
    }
}
