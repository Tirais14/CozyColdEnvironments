using R3;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public interface IDragHandler
    {
        bool IsDragging { get; }

        Observable<DragContext> ObserveBeginDrag();

        Observable<DragContext> ObserveDrag();

        Observable<DragContext> ObserveEndDrag();
    }

    public interface IDragHandler<TBeginEvent, TDragEvent, TEndEvent> : IDragHandler

        where TBeginEvent : EventBase<TBeginEvent>, new()
        where TDragEvent : EventBase<TDragEvent>, new()
        where TEndEvent : EventBase<TEndEvent>, new()
    {
        new Observable<DragContext<TBeginEvent>> ObserveBeginDrag();

        new Observable<DragContext<TDragEvent>> ObserveDrag();

        new Observable<DragContext<TEndEvent>> ObserveEndDrag();

        Observable<DragContext> IDragHandler.ObserveBeginDrag()
        {
            return ObserveBeginDrag().Select(ev => ev.AsUntyped());
        }

        Observable<DragContext> IDragHandler.ObserveDrag()
        {
            return ObserveDrag().Select(ev => ev.AsUntyped());
        }

        Observable<DragContext> IDragHandler.ObserveEndDrag()
        {
            return ObserveEndDrag().Select(ev => ev.AsUntyped());
        }
    }
}
