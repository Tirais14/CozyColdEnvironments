using CCEnvs.Unity.Components;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine.EventSystems;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public class DropHandler : CCBehaviour, IDropHandler
    {
        public event DragAndDropAction? OnDrop;

        public IObservable<PointerEventData> OnDragRx { get; private set; } = new Subject<PointerEventData>();

        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            OnDrop?.Invoke(eventData);
            ((Subject<PointerEventData>)OnDragRx).OnNext(eventData);
        }
    }
}
