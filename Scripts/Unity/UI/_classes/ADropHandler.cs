using CCEnvs.Diagnostics;
using CCEnvs.Unity.Components;
using CCEnvs.Unity.Injections;
using UnityEngine;
using UnityEngine.EventSystems;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public abstract class ADropHandler<TReciever>
        :
        CCBehaviour,
        IDropHandler

        where TReciever : Component, IView
    {
        [GetBySelf]
        protected TReciever recieverView;

        protected IDropHandler dropHandler;

        protected override void Start()
        {
            base.Start();

            dropHandler = (recieverView.GetViewModel() as IDropHandler)!;

            if (dropHandler.IsNull())
                throw new CCException($"View model must implements {nameof(IDropHandler)}.");
        }

        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            CCDebug.PrintLog($"{name} Dropped", new DebugContext(this).Additive());
            dropHandler.OnDrop(eventData);
        }
    }
}
