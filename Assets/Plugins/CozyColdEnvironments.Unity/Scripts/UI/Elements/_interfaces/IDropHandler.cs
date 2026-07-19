using R3;
using System;
using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public interface IDropHandler
    {
        GameObject gameObject { get; }

        void SendDropEvent<TEvent>(DragContext<TEvent> dragContext)
            where TEvent : EventBase<TEvent>, new();

        Observable<DropContext> ObserveDrop();
    }
}
