#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public interface IShowableElement : IShowableBase, IPaneledElement
    {
        IShowableElement? ShowableRoot { get; }

        IShowableElement? Parent { get; }

        IShowableElement[] GetDirectChilds();
    }
}
