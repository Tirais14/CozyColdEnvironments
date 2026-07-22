using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public interface IElement
    {
        PanelRenderer Renderer { get; }

        VisualElement? RendererRoot { get; }
    }
}
