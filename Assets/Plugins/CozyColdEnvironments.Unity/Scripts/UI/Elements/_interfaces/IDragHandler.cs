using R3;
using System;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public interface IDragHandler : IToggleable, IElement
    {
        event Action<DragContext> OnBeginDrag;
        event Action<DragContext> OnDrag;
        event Action<DragContext> OnEndDrag;

        bool IsDragging { get; }

        Observable<DragContext> ObserveBeginDrag();

        Observable<DragContext> ObserveDrag();

        Observable<DragContext> ObserveEndDrag();
    }
}
