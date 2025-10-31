using CCEnvs.Diagnostics;
using CCEnvs.Unity.Components;
using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public class DragHandler
        : CCBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        private Subject<PointerEventData>? onBeginDragSubj;
        private Subject<PointerEventData>? onDragSubj;
        private Subject<PointerEventData>? onEndDragSubj;

        public event DragAndDropAction? OnBeginDrag;
        public event DragAndDropAction? OnDrag;
        public event DragAndDropAction? OnEndDrag;

        protected override void Start()
        {
            base.Start();

            Init().Forget(ex => this.PrintException(ex));
        }

        private void OnDestroy()
        {
            foreach (var toggle in this.FindComponents<IDragAndDropTarget>())
                toggle.DeactivateDragAndDropAbility();
        }

        public static bool HasOn(GameObject gameObject)
        {
            CC.Guard.IsNotNull(gameObject, nameof(gameObject));

            return gameObject.GetComponent<DragHandler>();
        }
        public static bool HasOn(Component component)
        {
            CC.Guard.IsNotNull(component, nameof(component));

            return HasOn(component.gameObject);
        }

        public IObservable<PointerEventData> ObserveOnBeginDrag()
        {
            onBeginDragSubj ??= new Subject<PointerEventData>();

            return onBeginDragSubj;
        }

        public IObservable<PointerEventData> ObserveOnDrag()
        {
            onDragSubj ??= new Subject<PointerEventData>();

            return onDragSubj;
        }

        public IObservable<PointerEventData> ObserveOnEndDrag()
        {
            onEndDragSubj ??= new Subject<PointerEventData>();

            return onEndDragSubj;
        }

        private async UniTask Init()
        {
            await UniTask.WaitForEndOfFrame();

            foreach (var toggle in this.FindComponents<IDragAndDropTarget>())
                toggle.ActivateDragAndDropAbility();
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            OnBeginDrag?.Invoke(eventData);
            onBeginDragSubj?.OnNext(eventData);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            OnDrag?.Invoke(eventData);
            onDragSubj?.OnNext(eventData);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            OnEndDrag?.Invoke(eventData);
            onEndDragSubj?.OnNext(eventData);
        }
    }
}
