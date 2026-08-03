#nullable enable
using System;

namespace CCEnvs.UnityX.UI.Elements
{
    public interface IDragHandler : IToggleable
    {
        event Action<BeginDragEvent> OnBeginDrag;
        event Action<DragEvent> OnDrag;
        event Action<EndDragEvent> OnEndDrag;

        bool IsDragging { get; }

        IDragPredicate? Predicate { get; set; }
    }
}
