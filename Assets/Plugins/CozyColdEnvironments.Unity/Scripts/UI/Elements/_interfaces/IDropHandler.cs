using System;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public interface IDropHandler : IToggleable
    {
        event Action<DropEvent> OnDrop;

        GameObject gameObject { get; }

        void SendDropEvent(DragEvent dragContext);
    }
}
