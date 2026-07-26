using R3;
using System;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public interface IDragHandler : IToggleable, IElement
    {
        event DragAction OnBeginDrag;
        event DragAction OnDrag;
        event DragAction OnEndDrag;

        bool IsDragging { get; }

        IDragPredicate? Predicate { get; set; }
    }
}
