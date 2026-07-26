using R3;
using UnityEngine;
using System;

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
