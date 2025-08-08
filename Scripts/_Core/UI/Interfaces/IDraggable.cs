using System;
using UnityEngine.EventSystems;

#nullable enable
namespace UTIRLib.UI
{
    public interface IDraggable : IDragHandler, IEndDragHandler
    {
        event Action<PointerEventData> OnEndDragEvent;
    }
}
