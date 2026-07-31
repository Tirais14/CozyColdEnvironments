#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public interface IDragHandler : IToggleable
    {
        event DragAction OnBeginDrag;
        event DragAction OnDrag;
        event DragAction OnEndDrag;

        bool IsDragging { get; }

        IDragPredicate? Predicate { get; set; }
    }
}
