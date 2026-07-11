#nullable enable
using Cysharp.Threading.Tasks;
using System.Threading;
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

        IShowable? root { get; }

        IShowable? parent { get; }

        ICanvasController? canvasController { get; }

        IShowable[] GetDirectChilds();
    }
}
