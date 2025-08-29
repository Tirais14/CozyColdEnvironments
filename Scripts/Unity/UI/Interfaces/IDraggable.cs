using System;
using UnityEngine.EventSystems;

#nullable enable
namespace CozyColdEnvironments.UI
{
    public interface IDraggable : IDragHandler, IEndDragHandler
    {
        event Action<PointerEventData> OnEndDragEvent;
    }
}
