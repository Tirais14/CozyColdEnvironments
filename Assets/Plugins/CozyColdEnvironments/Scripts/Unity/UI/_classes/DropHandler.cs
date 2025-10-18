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
        private readonly List<IObserver<PointerEventData>> onDropObservers = new(0);

        public event DragAndDropAction? OnDrop;

        public IObservable<PointerEventData> OnDragRx { get; private set; }

        protected override void Start()
        {
            base.Start();

            OnDragRx = Observable.Create<PointerEventData>((x) =>
            {
                onDropObservers.Add(x);

                return Disposable.Create(() => onDropObservers.Remove(x));
            });
        }

        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            OnDrop?.Invoke(eventData);
            onDropObservers.ForEach(x => x.OnNext(eventData));
        }
    }
}
