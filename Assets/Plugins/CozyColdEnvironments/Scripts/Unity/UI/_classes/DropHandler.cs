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
    public class DropHandler : CCBehaviour, IDropHandler
    {
        private Subject<PointerEventData>? onDropSubj;

        public event DragAndDropAction? OnDrop;

        protected override void Start()
        {
            base.Start();
            Init().Forget(ex => this.PrintException(ex));
        }

        private void OnDestroy()
        {
            foreach (var toggle in this.GetAssignedObjects<IDragAndDropToggle>())
                toggle.DeactivateDragAndDropAbility();
        }

        public static bool HasOn(GameObject gameObject)
        {
            CC.Guard.NullArgument(gameObject, nameof(gameObject));

            return gameObject.TryGetComponent<DropHandler>(out _);
        }
        public static bool HasOn(Component component)
        {
            CC.Guard.NullArgument(component, nameof(component));

            return HasOn(component.gameObject);
        }

        public IObservable<PointerEventData> ObserveOnDrop()
        {
            onDropSubj ??= new Subject<PointerEventData>();

            return onDropSubj;
        }

        private async UniTask Init()
        {
            await UniTask.WaitForEndOfFrame();

            foreach (var toggle in this.GetAssignedObjects<IDragAndDropToggle>())
                toggle.ActivateDragAndDropAbility();
        }

        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            OnDrop?.Invoke(eventData);
            onDropSubj?.OnNext(eventData);
        }
    }
}
