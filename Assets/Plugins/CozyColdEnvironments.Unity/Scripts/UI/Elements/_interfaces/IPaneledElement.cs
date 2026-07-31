using System;
using UnityEngine.UIElements;

#nullable enable
namespace CCEnvs.UnityX.UI.Elements
{
    public interface IPaneledElement : IElement
    {
        PanelRenderer Renderer { get; }
    }
}
