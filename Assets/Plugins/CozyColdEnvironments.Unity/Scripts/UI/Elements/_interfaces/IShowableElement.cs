#nullable enable
using UnityEngine.UIElements;

namespace CCEnvs.UnityX.UI.Elements
{
    public interface IShowableElement : IShowableBase, IElement
    {
        IShowableElement? ShowableRoot { get; }

        IShowableElement? Parent { get; }

        IShowableElement[] GetDirectChilds();
    }
}
