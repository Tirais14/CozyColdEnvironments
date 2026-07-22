#nullable enable
using UnityEngine;
using UnityEngine.UI;

namespace CCEnvs.UnityX.UI
{
    public interface IShowable : IShowableBase
    {
        ShowableRenderMode RenderMode { get; set; }

        Graphic? graphic { get; }

        Image? image { get; }

        CanvasGroup? canvasGroup { get; }

        Canvas canvas { get; }

        IShowable? ShowableRoot { get; }

        IShowable? Parent { get; }

        ICanvasController? canvasController { get; }

        IShowable[] GetDirectChilds();
    }
}
