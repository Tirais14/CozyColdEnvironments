using CCEnvs.Unity.Components;
using CommunityToolkit.Diagnostics;
using R3;
using UnityEngine.EventSystems;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public class DragAndDropModule
        : 
        CCBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        private readonly ReactiveProperty<bool> isDragging = new();

        private ReactiveCommand<PointerEventData>? onBeginDragCallback;
        private ReactiveCommand<PointerEventData>? onDragCallback;
        private ReactiveCommand<PointerEventData>? onEndDragCallback;

        public bool IsDragging => isDragging.Value;

        public void OnBeginDrag(PointerEventData eventData)
        {
            Guard.IsNotNull(eventData, nameof(eventData));

            if (enabled)
                OnBeginDragCore(eventData);

            onBeginDragCallback?.Execute(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Guard.IsNotNull(eventData, nameof(eventData));

            if (enabled)
                OnDragCore(eventData);

            onDragCallback?.Execute(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Guard.IsNotNull(eventData, nameof(eventData));

            if (enabled)
                OnEndDrag(eventData);

            onEndDragCallback?.Execute(eventData);
        }

        public virtual bool CanBeDragged() => enabled;

        public Observable<PointerEventData> ObserveOnBeginDrag()
        {
            onBeginDragCallback ??= new ReactiveCommand<PointerEventData>();
            return onBeginDragCallback;
        }

        public Observable<PointerEventData> ObserveOnDrag()
        {
            onDragCallback ??= new ReactiveCommand<PointerEventData>();
            return onDragCallback;
        }

        public Observable<PointerEventData> ObserveOnEndDrag()
        {
            onEndDragCallback ??= new ReactiveCommand<PointerEventData>();
            return onEndDragCallback;
        }

        protected virtual void OnBeginDragCore(PointerEventData eventData)
        {
            isDragging.Value = true;
        }

        protected virtual void OnDragCore(PointerEventData eventData)
        {
            isDragging.Value = true;

            transform.position = eventData.position;
        }

        protected virtual void OnEndDragCore(PointerEventData eventData)
        {
            isDragging.Value = false;
        }
    }
}
