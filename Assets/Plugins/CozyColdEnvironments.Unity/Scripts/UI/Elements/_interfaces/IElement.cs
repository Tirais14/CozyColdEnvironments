using R3;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public interface IElement
    {
        VisualElement? RootElement { get; }

        Observable<VisualElement?> ObserveRootElement();
    }
}
