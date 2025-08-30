using CCEnvs.Diagnostics;
using CCEnvs.Unity.ComponentSetter;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public abstract class ADropHandler<TReciever>
        :
        MonoCC,
        IDropHandler

        where TReciever : Component, IView
    {
        [GetBySelf]
        protected TReciever recieverView;

        protected IDropHandler dropHandler;

        protected override void OnStart()
        {
            base.OnStart();

            dropHandler = (recieverView.GetViewModel() as IDropHandler)!;

            if (dropHandler.IsNull())
                throw new System.NullReferenceException($"View model must implements {nameof(IDropHandler)}.");
        }

        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            Debug.Log($"{name} Dropped");
            dropHandler.OnDrop(eventData);
        }
    }
}
