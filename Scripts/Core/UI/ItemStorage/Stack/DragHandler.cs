using UnityEngine.EventSystems;
using UTIRLib.Attributes;
using UTIRLib.Injector;

#nullable enable
namespace UTIRLib.UI
{
    public class DragHandler : MonoX,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        [GetComponent]
        [RequiredMember]
        private IMovable movable = null!;

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            movable.Position = eventData.pressPosition;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            movable.Position = eventData.position;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData _)
        {
            movable.ResetPosition();
        }
    }
}
