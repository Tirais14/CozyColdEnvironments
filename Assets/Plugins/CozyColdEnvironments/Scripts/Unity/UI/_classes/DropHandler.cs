using CCEnvs.Diagnostics;
using CCEnvs.Reflection;
using CCEnvs.Unity.Components;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using ZLinq;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public class DropHandler : CCBehaviour, IDropHandler
    {
        public event DragAndDropAction? OnDrop;

        public IObservable<PointerEventData> OnDropRx { get; private set; } = new Subject<PointerEventData>();

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

        private async UniTask Init()
        {
            await UniTask.WaitForEndOfFrame();

            foreach (var toggle in this.GetAssignedObjects<IDragAndDropToggle>())
                toggle.ActivateDragAndDropAbility();
        }

        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            OnDrop?.Invoke(eventData);
            ((Subject<PointerEventData>)OnDropRx).OnNext(eventData);
        }
    }
}
