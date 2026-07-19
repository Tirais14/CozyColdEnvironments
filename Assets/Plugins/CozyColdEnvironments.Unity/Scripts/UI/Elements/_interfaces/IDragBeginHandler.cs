using R3;
using System;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public interface IDragBeginHandler<TDownEvent> where TDownEvent : EventBase<TDownEvent>, new()
    {
        event Action<DragAndDropContext<TDownEvent>> OnBeginDrag;

        Observable<DragAndDropContext<TDownEvent>> ObserveBeginDrag();
    }
}
