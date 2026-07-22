using UnityEngine;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public interface IDragTarget
    {
        PanelRenderer Renderer { get; }

        VisualElement? RendererRoot { get; }

        string? tag { get; }

        int layer { get; }

        DragTarget ResetPosition();

        DragTarget SetPosition(Vector2 position);
    }
}
