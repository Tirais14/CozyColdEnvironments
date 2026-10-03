using R3;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public interface IPaneledElement : IElement
    {
        PanelRenderer? Renderer { get; }

        Observable<PanelRenderer?> ObserveRenderer();
    }
}
