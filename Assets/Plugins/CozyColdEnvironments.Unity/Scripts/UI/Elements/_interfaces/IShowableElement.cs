#nullable enable
using UnityEngine.UIElements;

namespace CCEnvs.UnityX.UI.Elements
{
    public interface IShowableElement : IShowableBase
    {
        IShowableElement? root { get; }

        IShowableElement? parent { get; }

        PanelRenderer renderer { get; }

        VisualElement? rendererRoot { get; }

        IShowableElement[] GetDirectChilds();
    }
}
