using UnityEngine;
using UnityEngine.EventSystems;
using UTIRLib.Diagnostics;

#nullable enable
namespace UTIRLib.UI
{
    public abstract class ADropHandler<TReciever>
        :
        MonoX,
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
