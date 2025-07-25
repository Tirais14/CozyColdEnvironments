using UnityEngine.EventSystems;
using UTIRLib.Attributes;

#pragma warning disable IDE0044
#nullable enable
namespace UTIRLib.UI
{
    public abstract class ADragHandler : MonoX,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        [RequiredField]
        private IMovable movable = null!;

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            movable.Position = eventData.pressPosition;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            movable.Position = eventData.position;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            movable.ResetPosition();
        }
    }
}
