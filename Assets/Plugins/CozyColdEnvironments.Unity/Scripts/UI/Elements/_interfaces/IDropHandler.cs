using R3;
using System;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public interface IDropHandler<TEvent> where TEvent : EventBase<TEvent>, new()
    {
        event Action<TEvent> OnDrop;

        Observable<TEvent> ObserveDrop();
    }
}
