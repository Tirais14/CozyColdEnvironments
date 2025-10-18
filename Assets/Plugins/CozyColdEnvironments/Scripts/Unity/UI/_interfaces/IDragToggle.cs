using System;
using UniRx;

#nullable enable
namespace CCEnvs.Unity.UI
{
    public interface IDragToggle
    {
        int DragAndDropHandlerBindingCount { get; }

        void ActivateDragAndDropAbility();

        void DeactivateDragAndDropAbility();
    }
}
