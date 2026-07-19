using R3;
using System;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public interface IDragHandler<TBeginEvent, TDragEvent, TEndEvent>

        where TBeginEvent : EventBase<TBeginEvent>, new()
        where TDragEvent : EventBase<TDragEvent>, new()
        where TEndEvent : EventBase<TEndEvent>, new()
    {
        Observable<DragAndDropContext<TBeginEvent>> ObserveBeginDrag();

        Observable<DragAndDropContext<TDragEvent>> ObserveDrag();

        Observable<DragAndDropContext<TEndEvent>> ObserveEndDrag();
    }
}
