using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.Items.UIElements
{
    public interface IViewElement
    {
        PanelRenderer Renderer { get; }

        VisualElement? RendererRoot { get; }
    }
}
