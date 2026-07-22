using R3;
using UnityEngine;
using System;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public interface IDropHandler : IToggleable
    {
        event Action<DropContext> OnDrop;

        GameObject gameObject { get; }

        void SendDropEvent(DragContext dragContext);

        Observable<DropContext> ObserveDrop();
    }
}
