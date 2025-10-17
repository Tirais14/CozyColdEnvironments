using CCEnvs.Unity.Components;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public delegate void DragAction(PointerEventData eventData);

    public class DragHandler
        : CCBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        private readonly List<IObserver<PointerEventData>> onBeginDragObservers = new(0);
        private readonly List<IObserver<PointerEventData>> onDragObservers = new(0);
        private readonly List<IObserver<PointerEventData>> onEndDragObservers = new(0);

        public event DragAction? OnBeginDrag;
        public event DragAction? OnDrag;
        public event DragAction? OnEndDrag;

        public IObservable<PointerEventData> OnBeginDragRx { get; private set; } = null!;
        public IObservable<PointerEventData> OnDragRx { get; private set; } = null!;
        public IObservable<PointerEventData> OnEndDragRx { get; private set; } = null!;

        protected override void Awake()
        {
            OnBeginDragRx = Observable.Create<PointerEventData>((x) =>
            {
                onBeginDragObservers.Add(x);

                return Disposable.Create(() => onBeginDragObservers.Remove(x));
            });

            OnDragRx = Observable.Create<PointerEventData>((x) =>
            {
                onDragObservers.Add(x);

                return Disposable.Create(() => onDragObservers.Remove(x));
            });

            OnEndDragRx = Observable.Create<PointerEventData>((x) =>
            {
                onEndDragObservers.Add(x);

                return Disposable.Create(() => onEndDragObservers.Remove(x));
            });
        }

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
            onBeginDragObservers.ForEach(x => x.OnNext(eventData));
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            OnDrag?.Invoke(eventData);
            onDragObservers.ForEach(x => x.OnNext(eventData));
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            OnEndDrag?.Invoke(eventData);
            onEndDragObservers.ForEach(x => x.OnNext(eventData));
        }
    }
}
