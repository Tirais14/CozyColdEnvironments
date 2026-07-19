using R3;
using UnityEngine;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public interface IDropHandler : IToggleable
    {
        GameObject gameObject { get; }

        void SendDropEvent(DragContext dragContext);

        Observable<DropContext> ObserveDrop();
    }
}
