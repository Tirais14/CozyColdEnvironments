using R3;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public interface IDragHandler : IToggleable
    {
        bool IsDragging { get; }

        Observable<DragContext> ObserveBeginDrag();

        Observable<DragContext> ObserveDrag();

        Observable<DragContext> ObserveEndDrag();
    }
}
